using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreator : ISessionCreator
    {
        private readonly ILogger _logger;
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IAuditor _auditor;
        private readonly IUserSessionManager _userSessionManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IServiceJourneyRulesService _serviceJourneyRules;

        public SessionCreator(
            ILogger<SessionCreator> logger,
            ICitizenIdSessionService citizenIdSessionService,
            IOdsCodeMassager odsCodeMassager,
            IAuditor auditor,
            IUserSessionManager userSessionManager,
            IUserInfoService userInfoService,
            IServiceJourneyRulesService serviceJourneyRules)
        {
            _logger = logger;
            _citizenIdSessionService = citizenIdSessionService;
            _odsCodeMassager = odsCodeMassager;
            _auditor = auditor;
            _userSessionManager = userSessionManager;
            _userInfoService = userInfoService;
            _serviceJourneyRules = serviceJourneyRules;
        }

        public async Task<CreateSessionResult> CreateSession(ICreateSessionRequest request)
        {
            // Call Citizen ID to get the User Profile (IM1 connection token, ODS code, Date of Birth, NHS Number).
            var citizenIdSessionResult =
                await _citizenIdSessionService.Create(request.AuthCode, request.CodeVerifier, request.RedirectUrl);

            if (!citizenIdSessionResult.StatusCode.IsSuccessStatusCode())
            {
                var errorTypes = citizenIdSessionResult.StatusCode switch
                {
                    // 502 Bad gateway error references differ by source API. The other error types do not.
                    StatusCodes.Status502BadGateway => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, StatusCodes.Status502BadGateway, SourceApi.NhsLogin),
                    _ => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, citizenIdSessionResult.StatusCode)
                };

                return new CreateSessionResult.Error(errorTypes);
            }

            citizenIdSessionResult.Session.OdsCode =
                _odsCodeMassager.CheckOdsCode(citizenIdSessionResult.Session.OdsCode);

            await _auditor.Audit()
                .AccessToken(citizenIdSessionResult.Session.AccessToken)
                .NhsNumber(citizenIdSessionResult.NhsNumber)
                .Supplier(Supplier.Unknown)
                .Operation(AuditingOperations.CitizenIdSessionCreate)
                .Details("Create Citizen Id Session")
                .Execute(() => Task.FromResult(citizenIdSessionResult));

            _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");

            return await GetServiceJourneyRulesAndCreateSession(request, citizenIdSessionResult);
        }

        private async Task<CreateSessionResult> GetServiceJourneyRulesAndCreateSession(
            ICreateSessionRequest request,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            // Get Service Journey Rules
            _logger.LogInformation(
                $"Retrieving Service Journey Rules for ods code: {citizenIdSessionResult.Session.OdsCode}");

            var serviceJourneyRulesResultVisited =
                await GetServiceJourneyRulesVisitorOutput(citizenIdSessionResult.Session.OdsCode);

            if (!serviceJourneyRulesResultVisited.ServiceJourneyRulesRetrieved)
            {
                var errorMessage =
                    $"Retrieving Service Journey Rules failed with status code: '{serviceJourneyRulesResultVisited.StatusCode}'";
                _logger.LogError(errorMessage);

                // Specific error reference for a 404 from SJR.
                var errorTypes = serviceJourneyRulesResultVisited.StatusCode switch
                {
                    StatusCodes.Status404NotFound => (ErrorTypes)new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported(),
                    _ => new ErrorTypes.LoginServiceJourneyRulesOtherError()
                };

                return new CreateSessionResult.Error(errorTypes);
            }

            var serviceJourneyRules = serviceJourneyRulesResultVisited.Response;

            var createUserSessionResult = await _userSessionManager.Create(citizenIdSessionResult, serviceJourneyRules, request.CsrfToken);

            return await createUserSessionResult.Accept(
                CreateErrorResult,
                success => CreateSession(request, success.UserSession, serviceJourneyRules, citizenIdSessionResult));

            static Task<CreateSessionResult> CreateErrorResult(CreateUserSessionResult.Failure error)
                => Task.FromResult<CreateSessionResult>(new CreateSessionResult.Error(error.ErrorType));
        }

        private async Task<CreateSessionResult> CreateSession(
            ICreateSessionRequest request,
            UserSession userSession,
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            // Post to the UserInfo service
            if (serviceJourneyRules.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(citizenIdSessionResult.Session.AccessToken, request.HttpContext);
            }

            return new CreateSessionResult.Success(serviceJourneyRules, userSession);
        }

        private async Task<ServiceJourneyRulesVisitorOutput> GetServiceJourneyRulesVisitorOutput(string odsCode)
        {
            var serviceJourneyRulesConfig = await _serviceJourneyRules.GetServiceJourneyRulesForOdsCode(odsCode);
            return serviceJourneyRulesConfig.Accept(new ServiceJourneyRulesConfigResultVisitor());
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
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
            var citizenIdSession = await FetchCitizenIdUserProfile(request);
            if (citizenIdSession.ProcessFinishedEarly(out var citizenIdSessionResult))
            {
                return citizenIdSessionResult;
            }

            var serviceJourneyRules = await FetchServiceJourneyRules(citizenIdSession.Result);
            if (serviceJourneyRules.ProcessFinishedEarly(out var serviceJourneyRulesResult))
            {
                return serviceJourneyRulesResult;
            }

            var userSession = await CreateUserSession(request, citizenIdSession.Result, serviceJourneyRules.Result);
            if (userSession.ProcessFinishedEarly(out var userSessionResult))
            {
                return userSessionResult;
            }

            await UpdateUserInfo(request, serviceJourneyRules.Result, citizenIdSession.Result);

            return new CreateSessionResult.Success(serviceJourneyRules.Result, userSession.Result);
        }

        private async Task<ProcessResult<CitizenIdSessionResult, CreateSessionResult>> FetchCitizenIdUserProfile(
            ICreateSessionRequest request)
        {
            var citizenIdSessionResult = await _citizenIdSessionService.Create(request.AuthCode, request.CodeVerifier, request.RedirectUrl);

            if (!citizenIdSessionResult.StatusCode.IsSuccessStatusCode())
            {
                var errorTypes = citizenIdSessionResult.StatusCode switch
                {
                    // 502 Bad gateway error references differ by source API. The other error types do not.
                    StatusCodes.Status502BadGateway => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, StatusCodes.Status502BadGateway, SourceApi.NhsLogin),
                    _ => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, citizenIdSessionResult.StatusCode)
                };

                return FinalResult<CitizenIdSessionResult>(errorTypes);
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

            return StepResult(citizenIdSessionResult);
        }

        private async Task<ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult>> FetchServiceJourneyRules(
            CitizenIdSessionResult citizenIdSessionResult)
        {
            var odsCode = citizenIdSessionResult.Session.OdsCode;

            _logger.LogInformation($"Retrieving Service Journey Rules for ods code: {odsCode}");
            var serviceJourneyRulesConfig = await _serviceJourneyRules.GetServiceJourneyRulesForOdsCode(odsCode);

            return serviceJourneyRulesConfig.Accept(new ServiceJourneyRulesConfigResultVisitor(_logger));
        }

        private async Task<ProcessResult<UserSession, CreateSessionResult>> CreateUserSession(
            ICreateSessionRequest request,
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesResponse serviceJourneyRules)
        {
            var createUserSessionResult = await _userSessionManager.Create(citizenIdSessionResult, serviceJourneyRules, request.CsrfToken);

            return createUserSessionResult.Accept(
                failure => FinalResult<UserSession>(failure.ErrorType),
                success => StepResult(success.UserSession));
        }

        private async Task UpdateUserInfo(
            ICreateSessionRequest request,
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            if (serviceJourneyRules.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(citizenIdSessionResult.Session.AccessToken, request.HttpContext);
            }
        }

        private static ProcessResult<TStepResult, CreateSessionResult> StepResult<TStepResult>(TStepResult result)
            => ProcessResult.StepResult<TStepResult, CreateSessionResult>(result);

        private static ProcessResult<TFinalResult, CreateSessionResult> FinalResult<TFinalResult>(ErrorTypes errorTypes)
            => ProcessResult.FinalResult<TFinalResult, CreateSessionResult>(new CreateSessionResult.Error(errorTypes));

        private sealed class ServiceJourneyRulesConfigResultVisitor
            : IServiceJourneyRulesConfigResultVisitor<ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult>>
        {
            private readonly ILogger _logger;

            public ServiceJourneyRulesConfigResultVisitor(ILogger logger) => _logger = logger;

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.Success result)
                => StepResult(result.Response);

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.NotFound result)
                => ErrorResult(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.InternalServerError result)
                => ErrorResult(new ErrorTypes.LoginServiceJourneyRulesOtherError());

            private ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> ErrorResult(ErrorTypes errorTypes)
            {
                var errorMessage = $"Retrieving Service Journey Rules failed with status code: '{errorTypes.StatusCode}'";
                _logger.LogError(errorMessage);
                
                return FinalResult<ServiceJourneyRulesResponse>(errorTypes);
            }
        }
    }
}
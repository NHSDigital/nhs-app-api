using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [ApiVersionRoute("session")]
    public class SessionController : Controller
    {
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly UserSessionService _userSessionService;
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<SessionController> _logger;
        private readonly IAuditor _auditor;
        private readonly IIm1CacheService _im1CacheService;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IServiceJourneyRulesService _serviceJourneyRules;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IUserInfoService _userInfoService;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IAntiforgery _antiforgery;
        private readonly IUserSessionManager _userSessionManager;

        public SessionController(
            ICitizenIdSessionService citizenIdSessionService,
            UserSessionService userSessionService,
            ConfigurationSettings settings,
            ILogger<SessionController> logger,
            IAuditor auditor,
            IIm1CacheService iIm1CacheService,
            IOdsCodeMassager odsCodeMassager,
            IServiceJourneyRulesService serviceJourneyRules,
            IErrorReferenceGenerator errorReferenceGenerator,
            IUserInfoService userInfoService,
            IGpSessionManager gpSessionManager,
            IAntiforgery antiforgery,
            IUserSessionManager userSessionManager)
        {
            _citizenIdSessionService = citizenIdSessionService;
            _userSessionService = userSessionService;
            _settings = settings;
            _logger = logger;
            _auditor = auditor;
            _im1CacheService = iIm1CacheService;
            _odsCodeMassager = odsCodeMassager;
            _serviceJourneyRules = serviceJourneyRules;
            _errorReferenceGenerator = errorReferenceGenerator;
            _userInfoService = userInfoService;
            _gpSessionManager = gpSessionManager;
            _antiforgery = antiforgery;
            _userSessionManager = userSessionManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                if (userSession is P9UserSession)
                {
                    await _auditor.Audit(AuditingOperations.SessionGetRequest, "Session Get called.");
                }

                var responseBody = userSession.Accept(new UserSessionResponseVisitor<UserSessionResponse>(_settings, new UserSessionResponse()));

                if (userSession is P9UserSession)
                {
                    await _auditor.Audit(AuditingOperations.SessionGetResponse, "Successfully retrieved session.");
                }

                return new OkObjectResult(responseBody);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var validator = new SessionValidator(_logger);

                if (!validator.IsPostValid(model))
                {
                    return BuildErrorResult(new ErrorTypes.LoginBadRequest());
                }

                return await GetCitizenIdSessionAndCreateSession(model);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<IActionResult> GetCitizenIdSessionAndCreateSession(UserSessionRequest model)
        {
            // Call Citizen ID to get the User Profile (IM1 connection token, ODS code, Date of Birth, NHS Number).
            var citizenIdSessionResult =
                await _citizenIdSessionService.Create(model.AuthCode, model.CodeVerifier, model.RedirectUrl);

            if (!citizenIdSessionResult.StatusCode.IsSuccessStatusCode())
            {
                // 502 Bad gateway error references differ by source API. The other error types do not.
                var objectResult = citizenIdSessionResult.StatusCode == StatusCodes.Status502BadGateway
                    ? BuildErrorResult(ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, StatusCodes.Status502BadGateway, SourceApi.NhsLogin))
                    : BuildErrorResult(ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, citizenIdSessionResult.StatusCode));

                return objectResult;
            }

            citizenIdSessionResult.Session.OdsCode = _odsCodeMassager.CheckOdsCode(citizenIdSessionResult.Session.OdsCode);

            _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");

            return await GetServiceJourneyRulesAndCreateSession(citizenIdSessionResult);
        }

        private async Task<IActionResult> GetServiceJourneyRulesAndCreateSession(CitizenIdSessionResult citizenIdSessionResult)
        {
            // Get Service Journey Rules
            _logger.LogInformation($"Retrieving Service Journey Rules for ods code: {citizenIdSessionResult.Session.OdsCode}");

            var serviceJourneyRulesResultVisited =
                await GetServiceJourneyRulesVisitorOutput(citizenIdSessionResult.Session.OdsCode);

            if (!serviceJourneyRulesResultVisited.ServiceJourneyRulesRetrieved)
            {
                var errorMessage =
                    $"Retrieving Service Journey Rules failed with status code: '{serviceJourneyRulesResultVisited.StatusCode}'";
                _logger.LogError(errorMessage);
                await _auditor.AuditSessionEvent(
                    citizenIdSessionResult.Session.AccessToken,
                    citizenIdSessionResult.NhsNumber,
                    Supplier.Unknown,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                // Specific error reference for a 404 from SJR.
                var objectResult = serviceJourneyRulesResultVisited.StatusCode == StatusCodes.Status404NotFound ?
                    BuildErrorResult(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported()):
                    BuildErrorResult(new ErrorTypes.LoginServiceJourneyRulesOtherError());

                return objectResult;
            }

            var serviceJourneyRules = serviceJourneyRulesResultVisited.Response;

            var createUserSessionResult = await _userSessionManager.Create(
                serviceJourneyRules,
                citizenIdSessionResult,
                _antiforgery.GetTokens(HttpContext).RequestToken);

            return await createUserSessionResult.Accept(
                error => Task.FromResult(BuildErrorResult(error.ErrorType)),
                success => CreateSession(success.UserSession, serviceJourneyRules, citizenIdSessionResult));
        }

        private async Task<IActionResult> CreateSession(
            UserSession userSession,
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            // Post to the UserInfo service
            if (serviceJourneyRules.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(citizenIdSessionResult.Session.AccessToken, HttpContext);
            }

            await AppendCookieToResponse(userSession.Key);

            _userSessionService.SetUserSession(userSession);

            // Audit that the user is logged on.
            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                serviceJourneyRules.Journeys.Supplier,
                AuditingOperations.SessionCreateResponse,
                "Session successfully created.");

            var responseBody = new PostUserSessionResponse { ServiceJourneyRules = serviceJourneyRules };
            responseBody = userSession.Accept(new UserSessionResponseVisitor<PostUserSessionResponse>(_settings, responseBody));
            return new CreatedResult(string.Empty, responseBody);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(AuditingOperations.SessionDeleteRequest, "Session delete called.");

                // Delete GP supplier session
                var gpUserSession = userSession.GpUserSession;
                var citizenIdUserSession = userSession.CitizenIdUserSession;

                var closeSessionResult = await _gpSessionManager.CloseAndDeleteSession(userSession);

                if (closeSessionResult is CloseSessionResult.Failure)
                {
                    await _auditor.AuditSessionEvent(
                        citizenIdUserSession.AccessToken,
                        gpUserSession.NhsNumber,
                        gpUserSession.Supplier,
                        AuditingOperations.SessionDeleteResponse,
                        "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogDebug(
                    $"Session successfully deleted. Finished with status code: {StatusCodes.Status204NoContent}");

                await _auditor.AuditSessionEvent(
                    citizenIdUserSession.AccessToken,
                    gpUserSession.NhsNumber,
                    gpUserSession.Supplier,
                    AuditingOperations.SessionDeleteResponse,
                    "Session successfully deleted");

                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IActionResult BuildErrorResult(ErrorTypes errorTypes)
        {
            var serviceDeskReference = _errorReferenceGenerator.GenerateAndLogErrorReference(errorTypes);

            return new ObjectResult(new PfsErrorResponse { ServiceDeskReference = serviceDeskReference })
            {
                StatusCode = errorTypes.StatusCode
            };
        }

        private async Task<ServiceJourneyRulesVisitorOutput> GetServiceJourneyRulesVisitorOutput(string odsCode)
        {
            var serviceJourneyRulesConfig = await _serviceJourneyRules.GetServiceJourneyRulesForOdsCode(odsCode);
            return serviceJourneyRulesConfig.Accept(new ServiceJourneyRulesConfigResultVisitor());
        }

        private async Task AppendCookieToResponse(string sessionId)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.SessionId, sessionId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );
        }

        private sealed class UserSessionResponseVisitor<TUserSessionResponse> : IUserSessionVisitor<TUserSessionResponse>
            where TUserSessionResponse: UserSessionResponse
        {
            private readonly ConfigurationSettings _settings;
            private readonly TUserSessionResponse _userSessionResponse;

            public UserSessionResponseVisitor(
                ConfigurationSettings settings,
                TUserSessionResponse userSessionResponse)
            {
                _settings = settings;
                _userSessionResponse = userSessionResponse;
            }

            public TUserSessionResponse Visit(P5UserSession userSession)
            {
                SetCommonProperties(userSession);
                _userSessionResponse.Name = $"{userSession.CitizenIdUserSession.GivenName} {userSession.CitizenIdUserSession.FamilyName}";
                _userSessionResponse.Im1MessagingEnabled = false;
                return _userSessionResponse;
            }

            public TUserSessionResponse Visit(P9UserSession userSession)
            {
                SetCommonProperties(userSession);
                _userSessionResponse.Name = userSession.GpUserSession.Name;
                _userSessionResponse.NhsNumber = userSession.GpUserSession.NhsNumber;
                _userSessionResponse.Im1MessagingEnabled = userSession.GpUserSession.Im1MessagingEnabled;
                return _userSessionResponse;
            }

            private void SetCommonProperties(P5UserSession userSession)
            {
                _userSessionResponse.SessionTimeout = (int)TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes).TotalSeconds;
                _userSessionResponse.OdsCode = userSession.OdsCode;
                _userSessionResponse.Token = userSession.CsrfToken;
                _userSessionResponse.DateOfBirth = userSession.CitizenIdUserSession.DateOfBirth;
                _userSessionResponse.AccessToken = userSession.CitizenIdUserSession.AccessToken;
                _userSessionResponse.ProofLevel = userSession.CitizenIdUserSession.ProofLevel;
            }
        }
    }
}
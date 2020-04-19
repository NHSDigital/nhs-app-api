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
using NHSOnline.Backend.GpSystems.Session;
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
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [ApiVersionRoute("session")]
    public class SessionController : Controller
    {
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly IGpSystemFactory _gpSystemFactory;
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
        private readonly ISessionCacheService _sessionCacheService;

        public SessionController(
            ICitizenIdSessionService citizenIdSessionService,
            IGpSystemFactory gpSystemFactory,
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
            ISessionCacheService sessionCacheService)
        {
            _citizenIdSessionService = citizenIdSessionService;
            _gpSystemFactory = gpSystemFactory;
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
            _sessionCacheService = sessionCacheService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(AuditingOperations.SessionGetRequest, "Session Get called.");

                var responseBody = new UserSessionResponse();

                PopulateUserSessionResponse(responseBody, userSession);

                await _auditor.Audit(AuditingOperations.SessionGetResponse, "Successfully retrieved session.");

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

            citizenIdSessionResult.OdsCode = _odsCodeMassager.CheckOdsCode(citizenIdSessionResult.OdsCode);

            _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");

            return await GetServiceJourneyRulesAndCreateSession(citizenIdSessionResult);
        }

        private async Task<IActionResult> GetServiceJourneyRulesAndCreateSession(CitizenIdSessionResult citizenIdSessionResult)
        {
            // Get Service Journey Rules
            _logger.LogInformation($"Retrieving Service Journey Rules for ods code: {citizenIdSessionResult.OdsCode}");

            var serviceJourneyRulesResultVisited =
                await GetServiceJourneyRulesVisitorOutput(citizenIdSessionResult.OdsCode);

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

            return await GetGpSystemAndCreateSession(citizenIdSessionResult, serviceJourneyRulesResultVisited);
        }
        private async Task<IActionResult> GetGpSystemAndCreateSession(CitizenIdSessionResult citizenIdSessionResult, ServiceJourneyRulesVisitorOutput serviceJourneyRulesResultVisited)
        {
            // Get a suitable GP system, based on the ODS code.
            var gpSystemOption = GetGpSystem(serviceJourneyRulesResultVisited.Response.Journeys.Supplier);
            if (!gpSystemOption.HasValue)
            {
                _logger.LogError(
                    $"Failed to determine the GP system based on ODS code '{citizenIdSessionResult.OdsCode}'");

                return BuildErrorResult(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());
            }

            var gpSystem = gpSystemOption.ValueOrFailure();
            _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");

            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                gpSystem.Supplier,
                AuditingOperations.SessionCreateRequest,
                "Attempting to create Session");

            // Validate the format of the IM1 connection token for this GP system.
            var tokenValidationService = gpSystem.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(citizenIdSessionResult.Im1ConnectionToken))
            {
                const string errorMessage = "Failed to validate Im1 connection";
                _logger.LogError(errorMessage);
                await _auditor.AuditSessionEvent(
                    citizenIdSessionResult.Session.AccessToken,
                    citizenIdSessionResult.NhsNumber,
                    gpSystem.Supplier,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                return BuildErrorResult(new ErrorTypes.LoginForbidden());
            }

            return await GetUserSessionAndCreateSession(gpSystem, citizenIdSessionResult, serviceJourneyRulesResultVisited);
        }

        private async Task<IActionResult> GetUserSessionAndCreateSession(IGpSystem gpSystem,
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesVisitorOutput serviceJourneyRulesResultVisited)
        {
            var gpSessionCreateResult = await _gpSessionManager.CreateSession(new GpSessionCreateArgs(gpSystem, citizenIdSessionResult));

            if (!(gpSessionCreateResult is GpSessionCreateResult.Success result))
            {
                var failureStatusCode = gpSessionCreateResult.StatusCode;

                var errorMessage =
                    $"Creating the session failed with status code: '{failureStatusCode}'";
                _logger.LogError(errorMessage);
                await _auditor.AuditSessionEvent(
                    citizenIdSessionResult.Session.AccessToken,
                    citizenIdSessionResult.NhsNumber,
                    gpSystem.Supplier,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                // 502 Bad gateway error references differ by supplier. The other error types do not.
                var objectResult = failureStatusCode == StatusCodes.Status502BadGateway
                    ? BuildErrorResult(ErrorTypes.LoginBadGateway(_logger, gpSystem.Supplier))
                    : BuildErrorResult(ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, failureStatusCode));

                return objectResult;
            }

            var userSession = new P9UserSession(
                _antiforgery.GetTokens(HttpContext).RequestToken,
                citizenIdSessionResult.Session,
                result.UserSession, citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");

            return await CreateSession(userSession, serviceJourneyRulesResultVisited, citizenIdSessionResult);
        }

        private async Task<IActionResult> CreateSession(P9UserSession userSession,
            ServiceJourneyRulesVisitorOutput serviceJourneyRulesVisitorOutput,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            var appendCookieToResponseTask = AppendCookieToResponse(userSession.Key);

            // Post to the UserInfo service
            if (serviceJourneyRulesVisitorOutput.Response.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(userSession.CitizenIdUserSession.AccessToken, HttpContext);
            }

            // Delete connection token from cache
            var tokenDeletionTask = DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);

            await Task.WhenAll(appendCookieToResponseTask, tokenDeletionTask);

            _userSessionService.SetUserSession(userSession);

            // Audit that the user is logged on.
            await _auditor.Audit(AuditingOperations.SessionCreateResponse, "Session successfully created.");

            return await Task.FromResult(CreateCreatedResult(userSession, serviceJourneyRulesVisitorOutput.Response));
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

        private CreatedResult CreateCreatedResult(
            P9UserSession userSession,
            ServiceJourneyRulesResponse serviceJourneyRules)
        {
            var responseBody = new PostUserSessionResponse
            {
                ServiceJourneyRules = serviceJourneyRules
            };

            PopulateUserSessionResponse(responseBody, userSession);

            return new CreatedResult(string.Empty, responseBody);
        }

        private ObjectResult BuildErrorResult(ErrorTypes errorTypes)
        {
            var serviceDeskReference = _errorReferenceGenerator.GenerateAndLogErrorReference(errorTypes);

            return new ObjectResult(new PfsErrorResponse { ServiceDeskReference = serviceDeskReference })
            {
                StatusCode = errorTypes.StatusCode
            };
        }

        private Option<IGpSystem> GetGpSystem(Supplier supplier)
        {
            return supplier == Supplier.Unknown ? Option.None<IGpSystem>() :
                Option.Some(_gpSystemFactory.CreateGpSystem(supplier));
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

        private async Task DeleteConnectionTokenFromCache(string im1ConnectionToken)
        {
            if (Guid.TryParse(im1ConnectionToken, out _))
            {
                return;
            }

            var tokenObject = JObject.Parse(im1ConnectionToken);

            if (tokenObject.TryGetValue(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName,
                StringComparison.Ordinal,
                out var cacheKey))
            {
                if (!string.IsNullOrEmpty(cacheKey?.ToString()))
                {
                    await _im1CacheService.DeleteIm1ConnectionToken(cacheKey.ToString());
                }
            }
        }

        private void PopulateUserSessionResponse(UserSessionResponse response, P9UserSession userSession)
        {
            response.Name = userSession.GpUserSession.Name;
            response.SessionTimeout = (int) TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes).TotalSeconds;
            response.Token = userSession.CsrfToken;
            response.OdsCode = userSession.GpUserSession.OdsCode;
            response.DateOfBirth = userSession.CitizenIdUserSession.DateOfBirth;
            response.NhsNumber = userSession.GpUserSession.NhsNumber;
            response.AccessToken = userSession.CitizenIdUserSession.AccessToken;
            response.Im1MessagingEnabled = userSession.GpUserSession.Im1MessagingEnabled;
            response.ProofLevel = userSession.CitizenIdUserSession.ProofLevel;
        }
    }
}
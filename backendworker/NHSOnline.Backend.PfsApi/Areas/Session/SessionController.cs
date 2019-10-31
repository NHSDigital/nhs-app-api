using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<SessionController> _logger;
        private readonly IAuditor _auditor;
        private readonly IIm1CacheService _im1CacheService;
        private readonly ISessionMapper _sessionMapper;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IServiceJourneyRulesService _serviceJourneyRules;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IUserInfoService _userInfoService;
        private readonly SessionConfigurationSettings _sessionSettings;

        public SessionController(
            ICitizenIdSessionService citizenIdSessionService,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            ConfigurationSettings settings,
            ILogger<SessionController> logger,
            IAuditor auditor,
            IIm1CacheService im1CacheService,
            ISessionMapper sessionMapper,
            IOdsCodeMassager odsCodeMassager,
            IServiceJourneyRulesService serviceJourneyRules,
            IErrorReferenceGenerator errorReferenceGenerator,
            SessionConfigurationSettings sessionSettings,
            IUserInfoService userInfoService
        )
        {
            _citizenIdSessionService = citizenIdSessionService;
            _gpSystemFactory = gpSystemFactory;
            _sessionCacheService = sessionCacheService;
            _odsCodeLookup = odsCodeLookup;
            _settings = settings;
            _logger = logger;
            _auditor = auditor;
            _im1CacheService = im1CacheService;
            _sessionMapper = sessionMapper;
            _odsCodeMassager = odsCodeMassager;
            _serviceJourneyRules = serviceJourneyRules;
            _errorReferenceGenerator = errorReferenceGenerator;
            _sessionSettings = sessionSettings;
            _userInfoService = userInfoService;
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
                    ? BuildErrorResult(ErrorCategory.Login, StatusCodes.Status502BadGateway,
                        SourceApi.NhsLogin)
                    : BuildErrorResult(ErrorCategory.Login, citizenIdSessionResult.StatusCode);

                return objectResult;
            }

            citizenIdSessionResult.OdsCode = _odsCodeMassager.CheckOdsCode(citizenIdSessionResult.OdsCode);

            _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");

            return await GetGpSystemAndCreateSession(citizenIdSessionResult);
        }

        private async Task<IActionResult> GetGpSystemAndCreateSession(CitizenIdSessionResult citizenIdSessionResult)
        {
            // Get a suitable GP system, based on the ODS code.
            var gpSystemOption = await GetGpSystem(citizenIdSessionResult.OdsCode);
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

            return await GetUserSessionAndCreateSession(gpSystem, citizenIdSessionResult);
        }

        private async Task<IActionResult> GetUserSessionAndCreateSession(IGpSystem gpSystem,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            // Create a session with the GP system, using the IM1 connection token.
            var gpSessionCreatedResultVisited = await GetGpSessionCreateResultVisitorOutput(gpSystem,
                citizenIdSessionResult.Im1ConnectionToken, citizenIdSessionResult.OdsCode,
                citizenIdSessionResult.NhsNumber);
            if (!gpSessionCreatedResultVisited.SessionWasCreated)
            {
                var errorMessage =
                    $"Creating the session failed with status code: '{gpSessionCreatedResultVisited.StatusCode}'";
                _logger.LogError(errorMessage);
                await _auditor.AuditSessionEvent(
                    citizenIdSessionResult.Session.AccessToken,
                    citizenIdSessionResult.NhsNumber,
                    gpSystem.Supplier,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                // 502 Bad gateway error references differ by supplier. The other error types do not.
                var objectResult = gpSessionCreatedResultVisited.StatusCode == StatusCodes.Status502BadGateway
                    ? BuildErrorResult(ErrorCategory.Login, StatusCodes.Status502BadGateway,
                        gpSystem.Supplier)
                    : BuildErrorResult(ErrorCategory.Login, gpSessionCreatedResultVisited.StatusCode);

                return objectResult;
            }

            var userSession = _sessionMapper.Map(HttpContext, gpSessionCreatedResultVisited.UserSession,
                citizenIdSessionResult.Session);

            return await GetServiceJourneyRulesAndCreateSession(userSession, citizenIdSessionResult,
                gpSessionCreatedResultVisited);
        }

        private async Task<IActionResult> GetServiceJourneyRulesAndCreateSession(UserSession userSession,
            CitizenIdSessionResult citizenIdSessionResult,
            GpSessionCreateResultVisitorOutput gpSessionCreatedResultVisited)
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
                    userSession.GpUserSession.Supplier,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                // Specific error reference for a 404 from SJR.
                var objectResult = serviceJourneyRulesResultVisited.StatusCode == StatusCodes.Status404NotFound ?
                    BuildErrorResult(new ErrorTypes.LoginServiceJourneyRulesOdsCodeNotFound()) :
                    BuildErrorResult(new ErrorTypes.LoginServiceJourneyRulesOtherError());

                return objectResult;
            }

            return await CreateSession(userSession, serviceJourneyRulesResultVisited, citizenIdSessionResult,
                gpSessionCreatedResultVisited);
        }

        private async Task<IActionResult> CreateSession(UserSession userSession,
            ServiceJourneyRulesVisitorOutput serviceJourneyRulesVisitorOutput,
            CitizenIdSessionResult citizenIdSessionResult,
            GpSessionCreateResultVisitorOutput gpSessionCreatedResultVisited)
        {
            // Build and save session token in our session cache
            var sessionFetchTask = FetchSessionIdAndSaveInCookie(userSession);

            // Post to the UserInfo service
            if (serviceJourneyRulesVisitorOutput.Response.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(userSession.CitizenIdUserSession.AccessToken);
            }

            // Delete connection token from cache
            var tokenDeletionTask = DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);

            await Task.WhenAll(sessionFetchTask, tokenDeletionTask);

            // Audit that the user is logged on.
            HttpContext.SetUserSession(userSession);
            await _auditor.Audit(AuditingOperations.SessionCreateResponse, "Session successfully created.");

            _logger.LogDebug($"Finished session post with status code {gpSessionCreatedResultVisited.StatusCode}");

            return await Task.FromResult(CreateCreatedResult(gpSessionCreatedResultVisited, userSession,
                serviceJourneyRulesVisitorOutput.Response, citizenIdSessionResult.DateOfBirth));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(AuditingOperations.SessionDeleteRequest, "Session delete called.");

                // Delete GP supplier session
                var userSession = HttpContext.GetUserSession();
                var gpUserSession = userSession.GpUserSession;
                var citizenIdUserSession = userSession.CitizenIdUserSession;

                try
                {
                    var supplierSessionDeletedResultVisited = await GetSessionLogoffResultVisitorOutput(userSession);
                    if (!supplierSessionDeletedResultVisited.SessionWasDeleted)
                    {
                        _logger.LogError(
                            $"Deleting the GP Supplier session failed with status code: '{supplierSessionDeletedResultVisited.StatusCode}'");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Deleting the GP supplier failed with error: {e.Message}");
                    await _auditor.AuditSessionEvent(
                        citizenIdUserSession.AccessToken,
                        gpUserSession.NhsNumber,
                        gpUserSession.Supplier,
                        AuditingOperations.SessionDeleteResponse,
                        "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                // Delete key and session
                bool sessionDeleted;
                try
                {
                    sessionDeleted = await _sessionCacheService.DeleteUserSession(userSession.Key);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Delete session failed with error: {e.Message}");
                    await _auditor.AuditSessionEvent(
                        citizenIdUserSession.AccessToken,
                        gpUserSession.NhsNumber,
                        gpUserSession.Supplier,
                        AuditingOperations.SessionDeleteResponse,
                        "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                if (!sessionDeleted)
                {
                    _logger.LogError("No active session was found");
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

        private CreatedResult CreateCreatedResult(GpSessionCreateResultVisitorOutput sessionCreatedResultVisited,
            UserSession userSession, ServiceJourneyRulesResponse serviceJourneyRules, DateTime dateOfBirth)
        {


            var responseBody = new UserSessionResponse
            {
                Name = sessionCreatedResultVisited.Name,
                SessionTimeout = _settings.DefaultSessionExpiryMinutes * 60,
                Token = userSession.CsrfToken,
                OdsCode = userSession.GpUserSession.OdsCode,
                DateOfBirth = dateOfBirth,
                NhsNumber = userSession.GpUserSession.NhsNumber,
                AccessToken = userSession.CitizenIdUserSession.AccessToken,
                ServiceJourneyRules = serviceJourneyRules,
            };

            return new CreatedResult(string.Empty, responseBody);
        }

        private async Task FetchSessionIdAndSaveInCookie(UserSession userSession)
        {
            // Build and save session token in our session cache
            var sessionId = await _sessionCacheService.CreateUserSession(userSession);

            _logger.LogDebug($"Fetched Session Id: '{sessionId}'");

            // Return the session token in a cookie.
            await AppendCookieToResponse(sessionId);
        }

        private static async Task<GpSessionCreateResultVisitorOutput> GetGpSessionCreateResultVisitorOutput(
            IGpSystem gpSystem,
            string im1ConnectionToken, string odsCode, string nhsNumber)
        {
            var sessionService = gpSystem.GetSessionService();
            var sessionCreateResult =
                await sessionService.Create(im1ConnectionToken, odsCode, nhsNumber);

            return sessionCreateResult.Accept(new GpSessionCreateResultVisitor());
        }

        private async Task<SessionLogoffResultVisitorOutput> GetSessionLogoffResultVisitorOutput(
            UserSession userSession)
        {
            var sessionService =
                _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier).GetSessionService();
            var sessionLogoffResult = await sessionService.Logoff(userSession.GpUserSession);

            return sessionLogoffResult.Accept(new SessionLogoffResultVisitor());
        }

        private ObjectResult BuildErrorResult(ErrorTypes errorTypes)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(errorTypes);

            return BuildErrorResult(serviceDeskReference, errorTypes.StatusCode);
        }

        private ObjectResult BuildErrorResult(ErrorCategory errorCategory, int statusCode, SourceApi sourceApi = SourceApi.None)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, sourceApi);

            return BuildErrorResult(serviceDeskReference, statusCode);
        }

        private ObjectResult BuildErrorResult(ErrorCategory errorCategory, int statusCode, Supplier supplier)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, supplier);

            return BuildErrorResult(serviceDeskReference, statusCode);
        }

        private static ObjectResult BuildErrorResult(string serviceDeskReference, int statusCode)
        {
            return new ObjectResult(new PfsErrorResponse
            {
                ServiceDeskReference = serviceDeskReference
            })
            {
                StatusCode = statusCode
            };
        }

        private async Task<Option<IGpSystem>> GetGpSystem(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            return !supplier.HasValue
                ? Option.None<IGpSystem>()
                : Option.Some(_gpSystemFactory.CreateGpSystem(supplier.ValueOrFailure()));
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
    }
}
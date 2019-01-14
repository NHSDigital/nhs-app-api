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
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session"), PfsSecurityMode]
    public class SessionController : Controller
    {
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly IOptions<ConfigurationSettings> _settings;
        private readonly ILogger<SessionController> _logger;
        private readonly IAuditor _auditor;
        private readonly IIm1CacheService _im1CacheService;
        private readonly ISessionMapper _sessionMapper;

        public SessionController(
            ICitizenIdSessionService citizenIdSessionService,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            IOptions<ConfigurationSettings> settings,
            ILogger<SessionController> logger,
            IAuditor auditor,
            IIm1CacheService im1CacheService,
            ISessionMapper sessionMapper
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
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            try
            {
                _logger.LogEnter();

                // Call Citizen ID to get the User Profile (IM1 connection token, ODS code, Date of Birth, NHS Number).
                var citizenIdSessionResult = await _citizenIdSessionService.Create(model.AuthCode, model.CodeVerifier, model.RedirectUrl);

                if (!citizenIdSessionResult.StatusCode.IsSuccessStatusCode())
                {
                    return new StatusCodeResult(citizenIdSessionResult.StatusCode);
                }
                
                // Get a suitable GP system, based on the ODS code.
                var gpSystemOption = await GetGpSystem(citizenIdSessionResult.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError($"Failed to determine the GP system based on ODS code '{citizenIdSessionResult.OdsCode}'");
                    return new StatusCodeResult(Constants.CustomHttpStatusCodes
                        .Status464OdsCodeNotSupportedOrNoNhsNumber);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");

                await _auditor.AuditWithExplicitNhsNumber(citizenIdSessionResult.NhsNumber, gpSystem.Supplier,
                    Constants.AuditingTitles.SessionCreateRequest,
                    "Attempting to create Session");

                // Validate the format of the IM1 connection token for this GP system.
                var tokenValidationService = gpSystem.GetTokenValidationService();
                if (!tokenValidationService.IsValidConnectionTokenFormat(citizenIdSessionResult.Im1ConnectionToken))
                {
                    const string errorMessage = "Failed to validate Im1 connection";
                    _logger.LogError(errorMessage);
                    await _auditor.AuditWithExplicitNhsNumber(citizenIdSessionResult.NhsNumber, gpSystem.Supplier,
                        Constants.AuditingTitles.SessionCreateResponse, errorMessage);
                    return new StatusCodeResult(StatusCodes.Status403Forbidden);
                }

                // Create a session with the GP system, using the IM1 connection token.
                var gpSessionCreatedResultVisited = await GetGpSessionCreateResultVisitorOutput(gpSystem, 
                    citizenIdSessionResult.Im1ConnectionToken, citizenIdSessionResult.OdsCode, citizenIdSessionResult.NhsNumber);
                if (!gpSessionCreatedResultVisited.SessionWasCreated)
                {
                    var errorMessage =
                        $"Creating the session failed with status code: '{gpSessionCreatedResultVisited.StatusCode}'";                    
                    _logger.LogError(errorMessage);
                    await _auditor.AuditWithExplicitNhsNumber(citizenIdSessionResult.NhsNumber, gpSystem.Supplier,
                        Constants.AuditingTitles.SessionCreateResponse, errorMessage);
                    return new StatusCodeResult(gpSessionCreatedResultVisited.StatusCode);
                }
                
                var userSession = _sessionMapper.Map(HttpContext, gpSessionCreatedResultVisited.UserSession, citizenIdSessionResult.Session);

                // Build and save session token in our session cache
                var sessionFetchTask = FetchSessionIdAndSaveInCookie(userSession);

                // Delete connection token from cache
                var tokenDeletionTask = DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);

                await Task.WhenAll(sessionFetchTask, tokenDeletionTask);

                // Audit that the user is logged on.
                HttpContext.SetUserSession(userSession);
                await _auditor.Audit(Constants.AuditingTitles.SessionCreateResponse, "Session successfully created.");

                _logger.LogDebug($"Finished session post with status code {gpSessionCreatedResultVisited.StatusCode}");
                
                _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");
                return await Task.FromResult(CreateCreatedResult(gpSessionCreatedResultVisited, userSession,
                    citizenIdSessionResult.DateOfBirth));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(Constants.AuditingTitles.SessionDeleteRequest, "Session delete called.");

                // Delete GP supplier session                
                var userSession = HttpContext.GetUserSession();
                var gpUserSession = userSession.GpUserSession;

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
                    await _auditor.AuditWithExplicitNhsNumber(gpUserSession.NhsNumber, gpUserSession.Supplier,
                        Constants.AuditingTitles.SessionDeleteResponse, "Delete session failed");

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
                    await _auditor.AuditWithExplicitNhsNumber(gpUserSession.NhsNumber, gpUserSession.Supplier,
                        Constants.AuditingTitles.SessionDeleteResponse, "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                if (!sessionDeleted)
                {
                    _logger.LogError("No active session was found");
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogDebug(
                    $"Session successfully deleted. Finished with status code: {StatusCodes.Status204NoContent}");

                await _auditor.AuditWithExplicitNhsNumber(gpUserSession.NhsNumber, gpUserSession.Supplier,
                    Constants.AuditingTitles.SessionDeleteResponse, "Session successfully deleted");

                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private CreatedResult CreateCreatedResult(GpSessionCreateResultVisitorOutput sessionCreatedResultVisited,
            UserSession userSession, DateTime dateOfBirth)
        {
            var responseBody = new UserSessionResponse
            {
                Name = sessionCreatedResultVisited.Name,
                SessionTimeout = _settings.Value.DefaultSessionExpiryMinutes * 60,
                Token = userSession.CsrfToken,
                OdsCode = userSession.GpUserSession.OdsCode,
                DateOfBirth = dateOfBirth,
                NhsNumber = userSession.GpUserSession.NhsNumber,
                AccessToken = userSession.CitizenIdUserSession.AccessToken
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

        private static async Task<GpSessionCreateResultVisitorOutput> GetGpSessionCreateResultVisitorOutput(IGpSystem gpSystem,
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
            var sessionService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier).GetSessionService();
            var sessionLogoffResult = await sessionService.Logoff(userSession);

            return sessionLogoffResult.Accept(new SessionLogoffResultVisitor());
        }

        private async Task<Option<IGpSystem>> GetGpSystem(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            return !supplier.HasValue
                ? Option.None<IGpSystem>()
                : Option.Some(_gpSystemFactory.CreateGpSystem(supplier.ValueOrFailure()));
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

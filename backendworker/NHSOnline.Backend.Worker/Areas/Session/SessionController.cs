using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Support.Session;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session"), PfsSecurityMode]
    public class SessionController : Controller
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly IOptions<ConfigurationSettings> _settings;
        private readonly ILogger<SessionController> _logger;
        private readonly IAuditor _auditor;
        private readonly IAntiforgery _antiforgery;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private const string DateFormat = "yyyy-MM-dd";

        public SessionController(
            ICitizenIdService citizenIdService,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            IOptions<ConfigurationSettings> settings,
            ILogger<SessionController> logger,
            IAuditor auditor,
            IAntiforgery antiforgery,
            IMinimumAgeValidator minimumAgeValidator
        )
        {
            _citizenIdService = citizenIdService;
            _gpSystemFactory = gpSystemFactory;
            _sessionCacheService = sessionCacheService;
            _odsCodeLookup = odsCodeLookup;
            _settings = settings;
            _logger = logger;
            _auditor = auditor;
            _antiforgery = antiforgery;
            _minimumAgeValidator = minimumAgeValidator;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            try
            {
                _logger.LogEnter(nameof(Post));

                // Call Citizen ID to get the User Profile (IM1 connection token, ODS code, Date of Birth, NHS Number).
                var userProfileResult = await _citizenIdService.GetUserProfile(model.AuthCode, model.CodeVerifier, model.RedirectUrl);
                var cidUserProfileOption = userProfileResult.UserProfile;

                if (!cidUserProfileOption.HasValue)
                {
                    _logger.LogError("No CID profile was found for received authcode and code verifier");
                    return new StatusCodeResult((int) userProfileResult.StatusCode);
                }

                var cidUserProfile = cidUserProfileOption.ValueOrFailure();

                // Validate the Date of Birth meets expected format and minimum age requirements
                if (!DateTime.TryParseExact(cidUserProfile.DateOfBirth, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateOfBirthParsed))
                {
                    _logger.LogError($"Missing or invalid date of birth");
                    return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement);
                }

                if (!_minimumAgeValidator.IsValid(dateOfBirthParsed))
                {
                    _logger.LogWarning("Failed to meet the minimum age requirement.");
                    return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement);
                }
                
                // Validate the NHS number has been returned from CID.
                var nhsNumberFormatted = cidUserProfile.NhsNumber.FormatToNhsNumber();
                if (string.IsNullOrEmpty(nhsNumberFormatted))
                {
                    _logger.LogError($"No NHS number was found");
                    return new StatusCodeResult(Constants.CustomHttpStatusCodes
                        .Status464OdsCodeNotSupportedOrNoNhsNumber);
                }

                // Get a suitable GP system, based on the ODS code.
                var gpSystemOption = await GetGpSystem(cidUserProfile.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError($"Failed to determine the GP system based on ODS code '{cidUserProfile.OdsCode}'");
                    return new StatusCodeResult(Constants.CustomHttpStatusCodes
                        .Status464OdsCodeNotSupportedOrNoNhsNumber);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");

                // Validate the format of the IM1 connection token for this GP system.
                var tokenValidationService = gpSystem.GetTokenValidationService();
                if (!tokenValidationService.IsValidConnectionTokenFormat(cidUserProfile.Im1ConnectionToken))
                {
                    _logger.LogError("Failed to validate Im1 connection");
                    return new StatusCodeResult(StatusCodes.Status403Forbidden);
                }

                // Create a session with the GP system, using the IM1 connection token.
                var sessionCreatedResultVisited = await GetSessionCreateResultVisitorOutput(gpSystem, 
                    cidUserProfile.Im1ConnectionToken, cidUserProfile.OdsCode, nhsNumberFormatted);
                if (!sessionCreatedResultVisited.SessionWasCreated)
                {
                    _logger.LogError(
                        $"Creating the session failed with status code: '{sessionCreatedResultVisited.StatusCode}'");
                    return new StatusCodeResult(sessionCreatedResultVisited.StatusCode);
                }

                // Build and save session token in our redis session cache
                await FetchSessionIdAndSaveInCookie(sessionCreatedResultVisited);

                // Audit that the user is logged on.
                HttpContext.SetUserSession(sessionCreatedResultVisited.UserSession);
                await _auditor.Audit(Constants.AuditingTitles.SessionCreateResponse, "Session successfully created.");

                _logger.LogDebug($"Finished session post with status code {sessionCreatedResultVisited.StatusCode}");

                return await Task.FromResult(CreateCreatedResult(sessionCreatedResultVisited, cidUserProfile.OdsCode,
                    dateOfBirthParsed, nhsNumberFormatted));
            }
            finally
            {
                _logger.LogExit(nameof(Post));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                _logger.LogEnter(nameof(Delete));
                await _auditor.Audit(Constants.AuditingTitles.SessionDeleteRequest, "Session delete called.");

                // Delete GP supplier session                
                var userSession = HttpContext.GetUserSession();

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
                    await _auditor.AuditWithExplicitNhsNumber(userSession.NhsNumber, userSession.Supplier,
                        Constants.AuditingTitles.SessionDeleteResponse, "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                // Delete redis key and session
                bool sessionDeleted;
                try
                {
                    sessionDeleted = await _sessionCacheService.DeleteUserSession(userSession.Key);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Delete session failed with error: {e.Message}");
                    await _auditor.AuditWithExplicitNhsNumber(userSession.NhsNumber, userSession.Supplier,
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

                await _auditor.AuditWithExplicitNhsNumber(userSession.NhsNumber, userSession.Supplier,
                    Constants.AuditingTitles.SessionDeleteResponse, "Session successfully deleted");

                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }
            finally
            {
                _logger.LogExit(nameof(Delete));
            }
        }

        private static CreatedResult CreateCreatedResult(SessionCreateResultVisitorOutput sessionCreatedResultVisited,
            string odsCode, DateTime dateOfBirth, string nhsNumber)
        {
            var responseBody = new UserSessionResponse
            {
                Name = sessionCreatedResultVisited.Name,
                SessionTimeout = sessionCreatedResultVisited.SessionTimeout,
                Token = sessionCreatedResultVisited.UserSession.CsrfToken,
                OdsCode = odsCode,
                DateOfBirth = dateOfBirth,
                NhsNumber = nhsNumber
            };

            return new CreatedResult(string.Empty, responseBody);
        }

        private async Task FetchSessionIdAndSaveInCookie(SessionCreateResultVisitorOutput sessionCreatedResultVisited)
        {
            // Generate CSRF token, to put into cache and return to browser
            sessionCreatedResultVisited.UserSession.CsrfToken =
                _antiforgery.GetTokens(HttpContext).RequestToken;

            // Build and save session token in our redis session cache
            var sessionId = await _sessionCacheService.CreateUserSession(sessionCreatedResultVisited.UserSession);

            _logger.LogDebug($"Fetched Session Id: '{sessionId}'");

            // Return the session token in a cookie.
            await AppendCookieToResponse(sessionId);
        }

        private async Task<SessionCreateResultVisitorOutput> GetSessionCreateResultVisitorOutput(IGpSystem gpSystem,
            string im1ConnectionToken, string odsCode, string nhsNumber)
        {
            var sessionService = gpSystem.GetSessionService();
            var sessionCreateResult =
                await sessionService.Create(im1ConnectionToken, odsCode, nhsNumber);

            return sessionCreateResult.Accept(new SessionCreateResultVisitor(_settings));
        }

        private async Task<SessionLogoffResultVisitorOutput> GetSessionLogoffResultVisitorOutput(
            UserSession userSession)
        {
            var sessionService = _gpSystemFactory.CreateGpSystem(userSession.Supplier).GetSessionService();
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
    }
}
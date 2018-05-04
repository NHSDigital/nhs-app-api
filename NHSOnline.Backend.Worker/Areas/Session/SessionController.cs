using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Session;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly ISystemProviderFactory _systemProviderFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            ICitizenIdService citizenIdService,
            ISystemProviderFactory systemProviderFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            ILoggerFactory loggerFactory)
        {
            _citizenIdService = citizenIdService;
            _systemProviderFactory = systemProviderFactory;
            _sessionCacheService = sessionCacheService;
            _odsCodeLookup = odsCodeLookup;
            _logger = loggerFactory.CreateLogger<SessionController>();
        }

        [HttpPost, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            _logger.LogDebug("Starting POST /session");
            // Call Citizen ID to get the IM1 connection token and ODS code.
            var cidUserProfileOption = await _citizenIdService.GetUserProfile(model.AuthCode, model.CodeVerifier);
            if (!cidUserProfileOption.HasValue)
            {
                _logger
                    .LogError("No CID profile was found for received authcode and code verifier");
                return BadRequest();
            }

            var cidUserProfile = cidUserProfileOption.ValueOrFailure();

            // Get an suitable GP system Provider, based on the ODS code.
            var systemProviderOption = await GetSystemProvider(cidUserProfile.OdsCode);
            if (!systemProviderOption.HasValue)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            var systemProvider = systemProviderOption.ValueOrFailure();

            // Validate the format of the IM1 connection token for this GP system.
            var tokenValidationService = systemProvider.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(cidUserProfile.Im1ConnectionToken))
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            // Create a session with the GP system, using the IM1 connection token.
            var sessionService = systemProvider.GetSessionService();
            var sessionCreateResult =
                await sessionService.Create(cidUserProfile.Im1ConnectionToken, cidUserProfile.OdsCode);

            var sessionCreatedResultVisited = sessionCreateResult.Accept(new SessionCreateResultVisitor());
            if (!sessionCreatedResultVisited.SessionWasCreated)
            {
                _logger
                    .LogError(
                        $"Creating the session failed with status code: {sessionCreatedResultVisited.StatusCode}");
                return new StatusCodeResult(sessionCreatedResultVisited.StatusCode);
            }

            // Build and save session token in our redis session cache
            var sessionId =
                await _sessionCacheService.CreateUserSession(
                    sessionCreatedResultVisited.UserSessionResponse.UserSession);

            // Return the session token in a cookie.
            await AppendCookieToResponse(sessionId);

            return await Task.FromResult(new CreatedResult(string.Empty,
                sessionCreatedResultVisited.UserSessionResponse));
        }

        private async Task<Option<ISystemProvider>> GetSystemProvider(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            if (!supplier.HasValue)
            {
                _logger.LogError($"Cannot find system provider for ODS code: {odsCode}");
                return Option.None<ISystemProvider>();
            }

            return Option.Some(_systemProviderFactory.CreateSystemProvider(supplier.ValueOrFailure()));
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
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public SessionController(
            ICitizenIdService citizenIdService,
            ISystemProviderFactory systemProviderFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            IConfiguration configuration)
        {
            _citizenIdService = citizenIdService;
            _systemProviderFactory = systemProviderFactory;
            _sessionCacheService = sessionCacheService;
            _odsCodeLookup = odsCodeLookup;
            _configuration = configuration;
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            // Call Citizen ID to get the IM1 connection token and ODS code.
            var cidUserProfileOption = await _citizenIdService.GetUserProfile(model.AuthCode, model.CodeVerifier);
            if (!cidUserProfileOption.HasValue)
            {
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
                return new StatusCodeResult(sessionCreatedResultVisited.StatusCode);
            }

            // Build and save session token in our redis session cache
            var sessionId = await CreateApiSession(sessionCreatedResultVisited, systemProvider);

            // Return the session token in a cookie.
            AppendCookieToResponse(sessionId);

            return await Task.FromResult(new CreatedResult(string.Empty,
                sessionCreatedResultVisited.UserSessionResponse));
        }

        private async Task<string> CreateApiSession(SessionCreateResultVisitorOutput sessionCreatedResultVisited,
            ISystemProvider systemProvider)
        {
            var userSession =
                new UserSession
                {
                    SupplierSessionId = sessionCreatedResultVisited.SupplierSessionId,
                    Supplier = systemProvider.Supplier
                };
            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            return sessionId;
        }

        private async Task<Option<ISystemProvider>> GetSystemProvider(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<ISystemProvider>();
            }

            return Option.Some(_systemProviderFactory.CreateSystemProvider(supplier.ValueOrFailure()));
        }

        private void AppendCookieToResponse(string sessionId)
        {
            var expires = DateTimeOffset.Now.AddMinutes(int.Parse(_configuration["SESSION_EXPIRY_MINUTES"]));
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expires
            };
            Response.Cookies.Append(Cookies.SessionId, sessionId, cookieOptions);
        }
    }
}
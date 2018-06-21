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
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ILogger<SessionController> _logger;
        private readonly IAuditor _auditor;

        public SessionController(
            ICitizenIdService citizenIdService,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService,
            IOdsCodeLookup odsCodeLookup,
            ILogger<SessionController> logger,
            IAuditor auditor)
        {
            _citizenIdService = citizenIdService;
            _gpSystemFactory = gpSystemFactory;
            _sessionCacheService = sessionCacheService;
            _odsCodeLookup = odsCodeLookup;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpPost, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            // Call Citizen ID to get the IM1 connection token and ODS code.
            var cidUserProfileOption = await _citizenIdService.GetUserProfile(model.AuthCode, model.CodeVerifier);
            if (!cidUserProfileOption.HasValue)
            {
                _logger.LogError("No CID profile was found for received authcode and code verifier");
                return BadRequest();
            }
            var cidUserProfile = cidUserProfileOption.ValueOrFailure();

            // Get a suitable GP system, based on the ODS code.
            var gpSystemOption = await GetGpSystem(cidUserProfile.OdsCode);
            if (!gpSystemOption.HasValue)
            {
                _logger.LogError($"Failed to determine the GP system based on ODS code '{cidUserProfile.OdsCode}'");
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
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
            var sessionCreatedResultVisited = await GetSessionCreateResultVisitorOutput(gpSystem, cidUserProfile);
            if (!sessionCreatedResultVisited.SessionWasCreated)
            {
                _logger.LogError($"Creating the session failed with status code: '{sessionCreatedResultVisited.StatusCode}'");
                return new StatusCodeResult(sessionCreatedResultVisited.StatusCode);
            }

            // Build and save session token in our redis session cache
            await FetchSessionIdAndSaveInCookie(sessionCreatedResultVisited);

            // Audit that the use is logged on.
            HttpContext.SetUserSession(sessionCreatedResultVisited.UserSession);
            _auditor.Audit("SessionCreation", "user session created");

            return await Task.FromResult(CreateCreatedResult(sessionCreatedResultVisited));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userSession = HttpContext.GetUserSession();
            
            var sessionDeleted = false;

            try
            {
                sessionDeleted = await _sessionCacheService.DeleteUserSession(userSession.Key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Delete session failed");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (!sessionDeleted)
            {
                _logger.LogError("No active session was found");
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        private CreatedResult CreateCreatedResult(SessionCreateResultVisitorOutput sessionCreatedResultVisited)
        {
            var responseBody = new UserSessionResponse
            {
                Name = sessionCreatedResultVisited.Name,
                SessionTimeout = sessionCreatedResultVisited.SessionTimeout
            };

            return new CreatedResult(string.Empty, responseBody);
        }

        private async Task FetchSessionIdAndSaveInCookie(SessionCreateResultVisitorOutput sessionCreatedResultVisited)
        {
            // Build and save session token in our redis session cache
            var sessionId = await _sessionCacheService.CreateUserSession(sessionCreatedResultVisited.UserSession);

            _logger.LogDebug($"Fetched Session Id: '{sessionId}'");

            // Return the session token in a cookie.
            await AppendCookieToResponse(sessionId);
        }

        private async Task<SessionCreateResultVisitorOutput> GetSessionCreateResultVisitorOutput(IGpSystem gpSystem, UserProfile cidUserProfile)
        {
            var sessionService = gpSystem.GetSessionService();
            var sessionCreateResult = await sessionService.Create(cidUserProfile.Im1ConnectionToken, cidUserProfile.OdsCode);
            return sessionCreateResult.Accept(new SessionCreateResultVisitor());
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
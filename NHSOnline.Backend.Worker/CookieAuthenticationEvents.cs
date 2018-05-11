using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Session;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ISessionCacheService _sessionCacheService;
        private ILogger<CustomCookieAuthenticationEvents> _logger;

        public CustomCookieAuthenticationEvents(ISessionCacheService sessionCacheService, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomCookieAuthenticationEvents>();
            _sessionCacheService = sessionCacheService;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            _logger.LogDebug("Start: Validate Principal");
            var userSession = await GetUserSession(context);

            if (!userSession.HasValue)
            {
                _logger.LogWarning("No user session found.  Signing out.");
                await RejectPrincipalAndSignOut(context);
            }

            _logger.LogWarning($"User session found: {userSession.ValueOrFailure()}");
            
            context.HttpContext.Items.Add(Constants.HttpContextItems.UserSession, userSession.ValueOrFailure());
            _logger.LogDebug("Finish: Validate Principal");
        }

        private async Task<Option<UserSession>> GetUserSession(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var sessionId = userPrincipal.Claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.SessionId)?.Value;

            if (string.IsNullOrEmpty(sessionId))
            {
                return Option.None<UserSession>();
            }

            var userSession = await _sessionCacheService.GetUserSession(sessionId);
            return userSession;
        }

        private static async Task RejectPrincipalAndSignOut(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly ILogger<CustomCookieAuthenticationEvents> _logger;

        public CustomCookieAuthenticationEvents(ISessionCacheService sessionCacheService, ILogger<CustomCookieAuthenticationEvents> logger)
        {
            _logger = logger;
            _sessionCacheService = sessionCacheService;
        }
        
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            using (_logger.BeginScope(context.HttpContext))
            {
                var userSessionOption = await GetUserSession(context);

                if (!userSessionOption.HasValue)
                {
                    _logger.LogWarning("No user session found. Signing out.");
                    await RejectPrincipalAndSignOut(context);
                    return;
                }
                var userSession = userSessionOption.ValueOrFailure();

                if (context.Request.Headers["X-CSRF-TOKEN"] != userSession.CsrfToken)
                {
                    _logger.LogWarning("Invalid X-CSRF-Token. Signing out.");
                    await RejectPrincipalAndSignOut(context);
                    return;
                }

                _logger.LogInformation($"User session found: '{userSession.GetType()}'");

                context.HttpContext.SetUserSession(userSession);
                _logger.LogDebug("Finish: Validate Principal");
            }
        }

        private async Task<Option<UserSession>> GetUserSession(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var sessionId = userPrincipal.Claims
                .FirstOrDefault(x => Constants.ClaimTypes.SessionId.Equals(x.Type, StringComparison.Ordinal))?.Value;

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
            _logger.LogDebug("Unauthorized request");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }
}

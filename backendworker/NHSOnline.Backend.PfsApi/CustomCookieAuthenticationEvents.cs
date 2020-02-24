using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<CustomCookieAuthenticationEvents> _logger;
        private readonly IGpSessionManager _gpSessionManager;

        public CustomCookieAuthenticationEvents(
            ILogger<CustomCookieAuthenticationEvents> logger,
            IGpSessionManager gpSessionManager)
        {
            _logger = logger;
            _gpSessionManager = gpSessionManager;
        }
        
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            using (_logger.BeginScope(context.HttpContext))
            {
                if (context.HttpContext.Request.Path.Equals("/v1/session", StringComparison.OrdinalIgnoreCase) &&
                    context.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                
                var retrieveSessionResult = await RetrieveSession(context);

                if (retrieveSessionResult is RetrieveSessionResult.Success success)
                {
                    context.HttpContext.SetUserSession(success.UserSession);
                    _logger.LogDebug("Finish: Validate Principal");
                    return;
                }

                await RejectPrincipalAndSignOut(context);
            }
        }

        private async Task<RetrieveSessionResult> RetrieveSession(CookieValidatePrincipalContext context)
        {
            var sessionId = context.Principal.Claims
                .FirstOrDefault(x => Constants.ClaimTypes.SessionId.Equals(x.Type, StringComparison.Ordinal))?.Value;

            var token = context.Request.Headers["X-CSRF-TOKEN"];

            return  await _gpSessionManager.RetrieveSession(sessionId, token);
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

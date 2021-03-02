using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<CustomCookieAuthenticationEvents> _logger;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly ISessionExpiryCookieCreator _sessionExpiryCookieCreator;
        private readonly ICurrentDateTimeProvider _currentDateTimeProvider;
        private readonly ConfigurationSettings _settings;

        public CustomCookieAuthenticationEvents(
            ILogger<CustomCookieAuthenticationEvents> logger,
            IGpSessionManager gpSessionManager,
            ISessionExpiryCookieCreator sessionExpiryCookieCreator,
            ICurrentDateTimeProvider currentDateTimeProvider,
            ConfigurationSettings settings)
        {
            _logger = logger;
            _gpSessionManager = gpSessionManager;
            _sessionExpiryCookieCreator = sessionExpiryCookieCreator;
            _currentDateTimeProvider = currentDateTimeProvider;
            _settings = settings;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.HttpContext.Request.Path.Equals("/v1/session", StringComparison.OrdinalIgnoreCase) &&
                context.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var (sessionExpiryToken, expiryTime) = GetSessionCookieAndExpiryTime(context);

            if (_currentDateTimeProvider.UtcNow < expiryTime)
            {
                RetrieveSessionResult retrieveSessionResult;
                var (sessionId, token) = GetSessionIdAndToken(context);

                if (IsUserMoreThanHalfWayIntoSession(expiryTime))
                {
                    // Recreate the JWT and update Cosmos session object
                    retrieveSessionResult = await _gpSessionManager.UpdateAndRetrieveSession(sessionId, token);
                    sessionExpiryToken = _sessionExpiryCookieCreator.CreateSessionExpiryToken();
                }
                else
                {
                    // No need to recreate the JWT or update the Cosmos session object
                    retrieveSessionResult = await _gpSessionManager.RetrieveSession(sessionId, token);
                }

                if (retrieveSessionResult is RetrieveSessionResult.Success success && !(sessionExpiryToken is null))
                {
                    context.HttpContext.RequestServices.GetRequiredService<UserSessionService>().SetUserSession(success.UserSession);

                    _sessionExpiryCookieCreator.AppendSessionExpiryCookie(context.HttpContext, sessionExpiryToken);

                    _logger.LogDebug("Finish: Validate Principal");
                    return;
                }
            }

            await RejectPrincipalAndSignOut(context);
        }

        private bool IsUserMoreThanHalfWayIntoSession(DateTime expiryTime)
        {
            var sessionExpiryHalfWayPoint = expiryTime.AddMinutes(-_settings.DefaultSessionExpiryMinutes / 2d);
            return _currentDateTimeProvider.UtcNow > sessionExpiryHalfWayPoint;
        }

        private DateTime GetExpiryTimeFromCookie(string sessionExpiryToken)
        {
            if (!string.IsNullOrEmpty(sessionExpiryToken))
            {
                var unencryptedCookie = _sessionExpiryCookieCreator.DecodeSessionExpiryToken(sessionExpiryToken);
                if (!string.IsNullOrEmpty(unencryptedCookie))
                {
                    var tokenObject = JObject.Parse(unencryptedCookie);

                    if (tokenObject.TryGetValue(JwtRegisteredClaimNames.Exp, StringComparison.Ordinal, out var exp))
                    {
                        return exp.ToObject<DateTime>();
                    }
                }
            }

            _logger.LogError("Could not extract Jwt claim 'Exp' from SessionExpiryToken");
            return default;
        }

        private (string sessionExpiryToken, DateTime expiryTime) GetSessionCookieAndExpiryTime(CookieValidatePrincipalContext context)
        {
            var sessionExpiryToken = context.Request.Cookies[Constants.CookieNames.SessionExpiry];
            var expiryTime = GetExpiryTimeFromCookie(sessionExpiryToken);

            return (sessionExpiryToken, expiryTime);
        }

        private (string sessionId, string token) GetSessionIdAndToken(CookieValidatePrincipalContext context)
        {
            var sessionId = context.Principal.Claims
                .FirstOrDefault(x => Constants.ClaimTypes.SessionId.Equals(x.Type, StringComparison.Ordinal))?.Value;

            _logger.LogInformation($"Session cookie present, retrieving sessionid={sessionId}");

            var token = context.Request.Headers["X-CSRF-TOKEN"];

            return (sessionId, token);
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

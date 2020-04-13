using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi
{
    public class UserSessionManager: IUserSessionManager
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;
        private readonly ILogger<UserSessionManager> _logger;

        public UserSessionManager(ISessionCacheService sessionCacheService, IAuditor auditor, ILogger<UserSessionManager> logger)
        {
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
            _logger = logger;
        }

        public async Task<bool> SignOutAsync(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var success = true;
            var userSession = httpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;
            var citizenIdUserSession = userSession.CitizenIdUserSession;

            try
            {
                if (!await _sessionCacheService.DeleteUserSession(userSession.Key))
                {
                    _logger.LogError("No active session was found");
                }
            }
            catch (Exception e)
            {
                success = false;
                _logger.LogError(e, $"Delete session failed with error: {e.Message}");
                await _auditor.AuditSessionEvent(
                    citizenIdUserSession.AccessToken,
                    gpUserSession.NhsNumber,
                    gpUserSession.Supplier,
                    AuditingOperations.SessionDeleteResponse,
                    "Delete session failed");
            }

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!success) return false;

            _logger.LogDebug("Session successfully deleted.");
            await _auditor.AuditSessionEvent(
                citizenIdUserSession.AccessToken,
                gpUserSession.NhsNumber,
                gpUserSession.Supplier,
                AuditingOperations.SessionDeleteResponse,
                "Session successfully deleted"
            );

            return true;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionDeleteCachedSessionStep : IUserSessionDeleteStep<UserSession>
    {
        private readonly ILogger _logger;
        private readonly ISessionCacheService _sessionCacheService;

        public UserSessionDeleteCachedSessionStep(
            ILogger<UserSessionDeleteCachedSessionStep> logger,
            ISessionCacheService sessionCacheService)
        {
            _logger = logger;
            _sessionCacheService = sessionCacheService;
        }

        public async Task<bool> Delete(HttpContext httpContext, UserSession userSession)
        {
            var userSessionDeleted = await _sessionCacheService.DeleteUserSession(userSession.Key);

            if (!userSessionDeleted)
            {
                _logger.LogWarning("No active session was found");
            }

            return userSessionDeleted;
        }
    }
}
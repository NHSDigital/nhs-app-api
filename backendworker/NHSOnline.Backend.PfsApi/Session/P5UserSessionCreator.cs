using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class P5UserSessionCreator
    {
        private readonly ILogger _logger;
        private readonly ISessionCacheService _sessionCacheService;

        public P5UserSessionCreator(ILogger<P5UserSessionCreator> logger, ISessionCacheService sessionCacheService)
        {
            _logger = logger;
            _sessionCacheService = sessionCacheService;
        }

        internal async Task<CreateUserSessionResult> Create(
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            var userSession = new P5UserSession(
                csrfToken,
                citizenIdSessionResult.Session);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");

            return CreateUserSessionResult.Succeeded(userSession);
        }
    }
}
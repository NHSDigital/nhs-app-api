using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;

namespace NHSOnline.Backend.PfsApi
{
    public class GpSessionManager : IGpSessionManager
    {
        private readonly ILogger<GpSessionManager> _logger;
        private readonly ISessionMapper _sessionMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionCacheService _sessionCacheService;
        
        public GpSessionManager(
            ILogger<GpSessionManager> logger,
            ISessionMapper sessionMapper, 
            IHttpContextAccessor httpContextAccessor,
            ISessionCacheService sessionCacheService
            )
        {
            _logger = logger;
            _sessionMapper = sessionMapper;
            _httpContextAccessor = httpContextAccessor;
            _sessionCacheService = sessionCacheService;
        }
        
        public async Task<CreateSessionResult> CreateSession(IGpSystem gpSystem,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            // Create a session with the GP system, using the IM1 connection token.
            var gpSessionCreateResult = await GetGpSessionCreateResult(
                gpSystem, citizenIdSessionResult.Im1ConnectionToken, 
                citizenIdSessionResult.OdsCode, citizenIdSessionResult.NhsNumber);

            if (!(gpSessionCreateResult is GpSessionCreateResult.Success result))
            {
                return new CreateSessionResult.Failure(gpSessionCreateResult.StatusCode); 
            }

            var userSession = _sessionMapper.Map(
                _httpContextAccessor.HttpContext, result.UserSession, 
                citizenIdSessionResult.Session, citizenIdSessionResult.Im1ConnectionToken);
            
            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Fetched Session Id: '{sessionId}'");
                   
            return new CreateSessionResult.Success(userSession);
        }

        public async Task<RetrieveSessionResult> RetrieveSession(string sessionId, StringValues csrfToken)  
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                _logger.LogWarning("Empty or null SessionId. Signing out.");
                return new RetrieveSessionResult.Failure();
            }

            var userSessionOption = await _sessionCacheService.GetUserSession(sessionId);
            
            if (!userSessionOption.HasValue)
            {
                _logger.LogWarning("No user session found. Signing out.");
                return new RetrieveSessionResult.Failure();
            }
            
            var userSession = userSessionOption.ValueOrFailure();

            if (csrfToken != userSession.CsrfToken)
            {
                _logger.LogWarning("Invalid X-CSRF-Token. Signing out.");
                return new RetrieveSessionResult.Failure();
            }

            _logger.LogInformation($"User session found: '{userSession.GetType()}'");
            return new RetrieveSessionResult.Success(userSession);
        }

        private static async Task<GpSessionCreateResult> GetGpSessionCreateResult(
            IGpSystem gpSystem, string im1ConnectionToken, string odsCode, string nhsNumber)
        {
            var sessionService = gpSystem.GetSessionService();
            return await sessionService.Create(im1ConnectionToken, odsCode, nhsNumber);
        }
    }
}
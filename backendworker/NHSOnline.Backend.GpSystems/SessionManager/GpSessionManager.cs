using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public class GpSessionManager : IGpSessionManager
    {
        private readonly ILogger<GpSessionManager> _logger;
        private readonly ISessionMapper _sessionMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IGpSystemFactory _gpSystemFactory;
        public GpSessionManager(
            ILogger<GpSessionManager> logger,
            ISessionMapper sessionMapper,
            IHttpContextAccessor httpContextAccessor,
            ISessionCacheService sessionCacheService,
            IGpSystemFactory gpSystemFactory)
        {
            _logger = logger;
            _sessionMapper = sessionMapper;
            _httpContextAccessor = httpContextAccessor;
            _sessionCacheService = sessionCacheService;
            _gpSystemFactory = gpSystemFactory;
        }

        public async Task<CreateSessionResult> CreateSession(
            IGpSystem gpSystem, GpSessionManagerCitizenIdSessionResult citizenIdSessionResult)
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

        public async Task<RecreateSessionResult> RecreateSession(string patientId)
        {
            // 1. retrieve session from cosmos
            var userSession = _httpContextAccessor.HttpContext.GetUserSession();
            var gpSystem = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier);
            var gpUserSession = userSession.GpUserSession;

            var connectionToken = userSession.Im1ConnectionToken;
            var odsCode = gpUserSession.OdsCode;
            var nhsNumber = gpUserSession.NhsNumber;

            // 2. create new session with provider
            var sessionService = gpSystem.GetSessionService();
            var recreateGpUserSessionResult = await sessionService.Recreate(connectionToken, odsCode, nhsNumber, patientId);

            if (recreateGpUserSessionResult is GpSessionRecreateResult.Success success)
            {
                // 3. merge new session - i.e. modify the GpUserSession
                var recreateSessionMapperService = gpSystem.GetRecreateSessionMapperService();
                var updatedGpUserSession = recreateSessionMapperService.Map(gpUserSession, success.Suid, patientId);

                // 4. update session in cosmos db
                userSession.GpUserSession = updatedGpUserSession;
                await _sessionCacheService.UpdateUserSession(userSession);

                return new RecreateSessionResult.Success(userSession);
            }
            return new RecreateSessionResult.Failure();
        }

        public async Task<CloseSessionResult> CloseAndDeleteSession(UserSession userSession)
        {
            var logoffSessionResult = await CloseSession(userSession.GpUserSession);

            if (logoffSessionResult is CloseSessionResult.Success)
            {
                return await DeleteSession(userSession.Key);
            }
            return new CloseSessionResult.Failure();
        }

        public async Task<CloseSessionResult> CloseSession(GpUserSession gpUserSession)
        {
            try
            {
                var sessionService = _gpSystemFactory.CreateGpSystem(gpUserSession.Supplier).GetSessionService();
                var logoffResult = await sessionService.Logoff(gpUserSession);

                var visitorResult  = logoffResult.Accept(new SessionLogoffResultVisitor());

                LogCloseSessionResult(gpUserSession, visitorResult);

                return new CloseSessionResult.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Deleting the GP supplier failed with error: {e.Message}");
                return new CloseSessionResult.Failure();
            }
        }

        private async Task<CloseSessionResult> DeleteSession(string key)
        {
            try
            {
                var sessionDeleted = await _sessionCacheService.DeleteUserSession(key);

                LogDeleteSessionResult(sessionDeleted, key);

                return new CloseSessionResult.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Delete session failed with error: {e.Message}");
                return new CloseSessionResult.Failure();
            }
        }

        private void LogDeleteSessionResult(bool sessionDeleted, string key)
        {
            if (sessionDeleted)
            {
                _logger.LogInformation("Successfully deleted the session from the cache.");
            }
            else
            {
                _logger.LogError($"No active session was found for {key} in session cache");
            }
        }

        private void LogCloseSessionResult(GpUserSession gpUserSession, SessionLogoffResultVisitorOutput visitorResult)
        {
            if (visitorResult.SessionWasDeleted)
            {
                _logger.LogInformation(
                    $"Successfully closed the {gpUserSession.Supplier.ToString()} GP Supplier session'");
            }
            else
            {
                _logger.LogError(
                    $"Deleting the GP Supplier session failed with status code: '{visitorResult.StatusCode}'");
            }
        }

        private static async Task<GpSessionCreateResult> GetGpSessionCreateResult(
            IGpSystem gpSystem, string im1ConnectionToken, string odsCode, string nhsNumber)
        {
            var sessionService = gpSystem.GetSessionService();
            return await sessionService.Create(im1ConnectionToken, odsCode, nhsNumber);
        }
    }
}
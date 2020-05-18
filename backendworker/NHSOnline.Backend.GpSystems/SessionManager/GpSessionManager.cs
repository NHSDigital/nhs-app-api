using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public class GpSessionManager : IGpSessionManager
    {
        private readonly ILogger<GpSessionManager> _logger;
        private readonly IUserSessionService _userSessionService;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IGpSystemFactory _gpSystemFactory;

        public GpSessionManager(
            ILogger<GpSessionManager> logger,
            IUserSessionService userSessionService,
            ISessionCacheService sessionCacheService,
            IGpSystemFactory gpSystemFactory)
        {
            _logger = logger;
            _userSessionService = userSessionService;
            _sessionCacheService = sessionCacheService;
            _gpSystemFactory = gpSystemFactory;
        }

        public async Task<GpSessionCreateResult> CreateSession(IGpSessionCreateArgs args)
        {
            var sessionService = args.GpSystem.GetSessionService();

            return await sessionService.Create(args.Im1ConnectionToken, args.OdsCode, args.NhsNumber);
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
            var userSession = _userSessionService.GetRequiredUserSession<P9UserSession>();

            var gpSystem = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier);
            var gpUserSession = userSession.GpUserSession;

            var sessionService = gpSystem.GetSessionService();
            var recreateGpUserSessionResult = await sessionService.Recreate(
                userSession.Im1ConnectionToken,
                gpUserSession.OdsCode,
                gpUserSession.NhsNumber,
                patientId);

            if (recreateGpUserSessionResult is GpSessionRecreateResult.Success success)
            {
                var recreateSessionMapperService = gpSystem.GetRecreateSessionMapperService();
                var updatedGpUserSession = recreateSessionMapperService.Map(gpUserSession, success.Suid, patientId);

                userSession.GpUserSession = updatedGpUserSession;
                await _sessionCacheService.UpdateUserSession(userSession);

                return new RecreateSessionResult.Success(userSession);
            }
            return new RecreateSessionResult.Failure();
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
    }
}
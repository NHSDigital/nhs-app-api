using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.GpSession.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpSessionCreator : IGpSessionCreator
    {
        private readonly ILogger<GpSessionCreator> _logger;
        private readonly IAuditor _auditor;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public GpSessionCreator(
            ILogger<GpSessionCreator> logger,
            IAuditor auditor,
            IGpSystemFactory gpSystemFactory,
            IGpSessionManager gpSessionManager,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _logger = logger;
            _auditor = auditor;
            _gpSystemFactory = gpSystemFactory;
            _gpSessionManager = gpSessionManager;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        public async Task CreateGpSession(
            CitizenIdSessionResult citizenIdUserSession, Supplier supplier, P9UserSession p9UserSession)
        {
            var gpSessionCreateArgs = new GpSessionCreateArgs(citizenIdUserSession);

            var createResult = await CreateGpSession(gpSessionCreateArgs, supplier);

            await createResult.Accept(
                new GpUserSessionCreateResultVisitor(
                    _logger,
                    supplier,
                    p9UserSession,
                    _errorReferenceGenerator));
        }

        public async Task<GpSessionRecreateResult> RecreateGpSession(
            P9UserSession userSession,
            Supplier supplier)
        {
            return await _auditor.Audit()
                .AccessToken(userSession.CitizenIdUserSession.AccessToken)
                .NhsNumber(userSession.NhsNumber)
                .Supplier(userSession.GpUserSession.Supplier)
                .Operation(AuditingOperations.GpSessionRecreate)
                .Details("Attempting to recreate P9 User GP Session")
                .Execute(() => DoRecreateGpSession(userSession, supplier));
        }

        private async Task<GpSessionRecreateResult> DoRecreateGpSession(
            P9UserSession userSession,
            Supplier supplier)
        {
            _logger.LogInformation("Attempting to recreate GP user session for P9 user");

            var gpSessionCreateArgs = new GpSessionRecreateArgs(
                userSession.Im1ConnectionToken,
                userSession.OdsCode,
                userSession.NhsNumber);

            var gpSession = await CreateGpSession(gpSessionCreateArgs, supplier);

            await gpSession.Accept(new GpUserSessionCreateResultVisitor(
                _logger, supplier, userSession, _errorReferenceGenerator));

            await _sessionCacheService.UpdateUserSession(userSession);

            return gpSession.Accept(new GpUserSessionRecreateResultVisitor(_logger, supplier, userSession.OdsCode));
        }

        private async Task<GpSessionCreateResult> CreateGpSession(
            IGpSessionCreateArgs gpSessionCreateArgs, Supplier supplier)
        {
            var gpSystem = FetchGpSystem(supplier);

            return await _gpSessionManager.CreateSession(gpSystem, gpSessionCreateArgs);
        }

        private IGpSystem FetchGpSystem(Supplier supplier)
        {
            var gpSystem = _gpSystemFactory.CreateGpSystem(supplier);

            _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");

            return gpSystem;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionClearDownVisitor : IGpUserSessionVisitor<Task>
    {
        private readonly ILogger _logger;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly P9UserSession _p9UserSession;

        public GpUserSessionClearDownVisitor(
            ILogger logger,
            IErrorReferenceGenerator errorReferenceGenerator,
            ISessionCacheService sessionCacheService,
            P9UserSession p9UserSession)
        {
            _logger = logger;
            _p9UserSession = p9UserSession;
            _errorReferenceGenerator = errorReferenceGenerator;
            _sessionCacheService = sessionCacheService;
        }

        public Task Visit(NullGpSession nullGpSession)
        {
            _logger.LogWarning($"No GP session detected for P9 user. odsCode={_p9UserSession.OdsCode}");

            return Task.CompletedTask;
        }

        public async Task Visit(GpUserSession gpSession)
        {
            var supplier = _p9UserSession.GpUserSession.Supplier;

            _logger.LogInformation(
                "Retrieved GP User Session for P9 user. " +
                $"supplier={supplier} odsCode={_p9UserSession.OdsCode}");

            var errorResult = new ErrorTypes.GPSessionUnavailable();
            var errorReference = _errorReferenceGenerator.GenerateAndLogErrorReference(errorResult);

            _p9UserSession.GpUserSession = new NullGpSession(supplier, errorReference);

            await _sessionCacheService.UpdateUserSession(_p9UserSession);
        }
    }
}
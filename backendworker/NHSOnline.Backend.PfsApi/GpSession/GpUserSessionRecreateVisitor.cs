using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpSessionRecreateVisitor : IGpUserSessionVisitor<Task<GpSessionRecreateResult>>
    {
        private readonly ILogger _logger;
        private readonly IGpSessionCreator _gpSessionCreator;
        private readonly P9UserSession _p9UserSession;

        public GpSessionRecreateVisitor(
            ILogger logger,
            IGpSessionCreator gpSessionCreator,
            P9UserSession p9UserSession)
        {
            _gpSessionCreator = gpSessionCreator;
            _p9UserSession = p9UserSession;
            _logger = logger;
        }

        public async Task<GpSessionRecreateResult> Visit(NullGpSession nullGpSession)
        {
            _logger.LogInformation("Invalid GP session detected, attempting to recreate GP user session");

            return await _gpSessionCreator.RecreateGpSession(_p9UserSession, nullGpSession.SessionSupplier);
        }

        public Task<GpSessionRecreateResult> Visit(GpUserSession gpSession)
        {
            _logger.LogInformation("Valid GP session present");

            return Task.FromResult<GpSessionRecreateResult>(new GpSessionRecreateResult.SessionStillValidResult());
        }
    }
}
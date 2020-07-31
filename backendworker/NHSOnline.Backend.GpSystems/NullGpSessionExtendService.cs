using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public class NullGpSessionExtendService : ISessionExtendService
    {
        private ILogger _logger;

        public NullGpSessionExtendService(ILogger logger)
        {
            _logger = logger;
        }

        public Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogInformation($"No GP session available, ignoring GP session extend request");

            return Task.FromResult<SessionExtendResult>(new SessionExtendResult.Success());
        }
    }
}

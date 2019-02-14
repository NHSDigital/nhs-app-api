using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session
{
    public class MicrotestSessionService : ISessionService
    {
        private readonly ILogger<MicrotestSessionService> _logger;
        
        public MicrotestSessionService(ILogger<MicrotestSessionService> logger)
        {
            _logger = logger;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            var session = await Task.FromResult(new MicrotestUserSession
            {
                NhsNumber = nhsNumber.RemoveWhiteSpace(),
                OdsCode = odsCode,
            });

            // TODO
            // Retrieve name from Microtest when endpoint is available.
            return new GpSessionCreateResult.SuccessfullyCreated("Microtest user", session);
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(gpUserSession));
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Session
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

        public Task<SessionLogoffResult> Logoff(UserSession userSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(userSession));
        }
    }
}

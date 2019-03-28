using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session
{
    public class MicrotestSessionService : ISessionService
    {
        private readonly ILogger<MicrotestSessionService> _logger;
        private readonly IMicrotestDemographicsService _demographicsService;

        public MicrotestSessionService(
            ILogger<MicrotestSessionService> logger,
            IMicrotestDemographicsService demographicsService)
        {
            _logger = logger;
            _demographicsService = demographicsService;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            var session = await Task.FromResult(new MicrotestUserSession
            {
                NhsNumber = nhsNumber,
                OdsCode = odsCode
            });
            
            var demographicsResult = await _demographicsService.GetDemographics(session);
            
            if (!(demographicsResult is DemographicsResult.SuccessfullyRetrieved successfulDemographicsResult))
            {
                _logger.LogError("Error retrieving demographics when creating session");
                return new GpSessionCreateResult.SupplierSystemUnavailable();
            }

            return new GpSessionCreateResult.SuccessfullyCreated(successfulDemographicsResult.Response.PatientName, session);
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(gpUserSession));
        }
    }
}

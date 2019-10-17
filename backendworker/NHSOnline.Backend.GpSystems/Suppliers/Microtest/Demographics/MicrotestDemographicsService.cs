using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics
{
    public class MicrotestDemographicsService : IMicrotestDemographicsService
    {
        private readonly ILogger<MicrotestDemographicsService> _logger;
        private readonly IMicrotestClient _microtestClient;
        private readonly IMicrotestDemographicsMapper _microtestDemographicsMapper;

        public MicrotestDemographicsService(
            ILoggerFactory loggerFactory, 
            IMicrotestClient microtestClient,
            IMicrotestDemographicsMapper microtestDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<MicrotestDemographicsService>();
            _microtestClient = microtestClient;
            _microtestDemographicsMapper = microtestDemographicsMapper;
        }

        public async Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();
            var microtestUserSession = (MicrotestUserSession) gpLinkedAccountModel.GpUserSession;

            try
            {
                var demographicsResponse = await _microtestClient.DemographicsGet(
                    microtestUserSession.OdsCode,
                    microtestUserSession.NhsNumber);

                if (!demographicsResponse.HasSuccessResponse)
                {
                    if (demographicsResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        _logger.LogWarning("User does not have the necessary permissions within the GP system.");
                        return new DemographicsResult.Forbidden();
                    }

                    _logger.LogError($"Unsuccessful request retrieving demographics. Status code: {(int)demographicsResponse.StatusCode}");
                    return new DemographicsResult.BadGateway();
                }

                _logger.LogInformation("Mapping Microtest responses to universal DemographicsResponse class instance");
                var result = _microtestDemographicsMapper.Map(demographicsResponse.Body);

                return new DemographicsResult.Success(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving demographics");
                return new DemographicsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}

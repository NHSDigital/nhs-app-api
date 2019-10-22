using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesService: IServiceJourneyRulesService
    {
        private readonly IServiceJourneyRulesClient _serviceJourneyRulesClient;
        private readonly ILogger<ServiceJourneyRulesService> _logger;

        public ServiceJourneyRulesService(IServiceJourneyRulesClient serviceJourneyRulesClient,  ILogger<ServiceJourneyRulesService> logger)
        {
            _serviceJourneyRulesClient = serviceJourneyRulesClient;
            _logger = logger;
        }

        public async Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForOdsCode(string odsCode)
        {
            _logger.LogEnter();
            try
            {
                var result = await _serviceJourneyRulesClient.GetServiceJourneyRules(odsCode);

                if (result.HasSuccessResponse && result.Body != null)
                {
                    return new ServiceJourneyRulesConfigResult.Success(result.Body);
                }

                _logger.LogError($"Failed to retrieve Service Journey Rules for ods code: {odsCode}");
                return new ServiceJourneyRulesConfigResult.NotFound();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Exception calling Service Journey Rules API, {ex}");
                return new ServiceJourneyRulesConfigResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
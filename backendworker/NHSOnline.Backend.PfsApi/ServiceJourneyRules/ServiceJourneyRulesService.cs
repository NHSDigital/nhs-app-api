using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesService: IServiceJourneyRulesService 
    {
        private readonly IServiceJourneyRulesClient _serviceJourneyRulesClient;
        private readonly ILogger<ServiceJourneyRulesService> _logger;
        
        private const string AppointmentsJourney = "im1Appointments";
        
        public ServiceJourneyRulesService(IServiceJourneyRulesClient serviceJourneyRulesClient,  ILogger<ServiceJourneyRulesService> logger)
        {
            _serviceJourneyRulesClient = serviceJourneyRulesClient;
            _logger = logger;
        }

        public async Task<bool> IsJourneyEnabled(string odsCode)
        {
            using (_logger.WithTimer($"Checking SJR Api to see if Journey is enabled for {odsCode}"))
            {
                try
                {
                    var result = await _serviceJourneyRulesClient.GetServiceJourneyRules(odsCode);

                    if (result.HasSuccessResponse && result.Body != null)
                    {
                        return result.Body.Appointments.JourneyType.ToString()
                            .Equals(AppointmentsJourney, StringComparison.OrdinalIgnoreCase);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception calling Service Journey Rules API, {ex}");
                    throw;
                }                
            }
        }
        
        public async Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForOdsCode(string odsCode)
        {
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
            catch (Exception ex)
            {
                _logger.LogError($"Exception calling Service Journey Rules API, {ex}");
                return new ServiceJourneyRulesConfigResult.BadGateway();
            }  
        }
    }
}
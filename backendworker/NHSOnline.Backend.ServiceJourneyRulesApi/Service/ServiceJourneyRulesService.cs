using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal class ServiceJourneyRulesService : IServiceJourneyRulesService
    {
        private readonly ILogger<ServiceJourneyRulesService> _logger;
        private readonly IJourneyRepository _journeyRepository;

        public ServiceJourneyRulesService(ILoggerFactory loggerFactory,
            IJourneyRepository journeyRepository)
        {
            _logger = loggerFactory.CreateLogger<ServiceJourneyRulesService>();
            _journeyRepository = journeyRepository;
        }

        public ServiceJourneyRulesResponse GetServiceJourneyRulesForOdsCode(string odsCode)
        {
            _logger.LogEnter();
            try
            {
                var journeys = _journeyRepository.GetJourneys(odsCode);

                if (journeys == null)
                {
                    _logger.LogInformation($"No Journeys returned from Journey Repository for ods code: {odsCode}");
                }
                return new ServiceJourneyRulesResponse { Journeys = journeys };
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}

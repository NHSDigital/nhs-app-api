using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesConfig: IServiceJourneyRulesConfig
    {        
        public Uri ServiceJourneyRulesBaseUrl { get; set; }
        
        public ServiceJourneyRulesConfig(IConfiguration configuration, ILogger<ServiceJourneyRulesConfig> logger)
        {
            var serviceJourneyRulesUri = configuration.GetOrWarn("SERVICE_JOURNEY_RULES_BASE_URL", logger);
            ServiceJourneyRulesBaseUrl = new Uri(serviceJourneyRulesUri, UriKind.Absolute);
        }
    }
}
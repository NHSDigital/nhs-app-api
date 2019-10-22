using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
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
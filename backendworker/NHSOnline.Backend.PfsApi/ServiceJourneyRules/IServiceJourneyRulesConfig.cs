using System;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesConfig
    {
        Uri ServiceJourneyRulesBaseUrl { get; set; }
    }
}
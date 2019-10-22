using System;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public interface IServiceJourneyRulesConfig
    {
        Uri ServiceJourneyRulesBaseUrl { get; set; }
    }
}
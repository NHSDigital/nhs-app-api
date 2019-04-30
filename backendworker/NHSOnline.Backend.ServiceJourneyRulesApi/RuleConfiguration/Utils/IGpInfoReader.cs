using System.Collections.Generic;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IGpInfoReader
    {
        IDictionary<string, GpInfo> GetGpInfo(string filePath);
    }
}
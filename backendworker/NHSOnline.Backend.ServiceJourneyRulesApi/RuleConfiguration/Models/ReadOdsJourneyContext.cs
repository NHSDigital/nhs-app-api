using System.Collections.Generic;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class LoadContext
    {
        public FileData TargetSchema { get; set; }
        
        public IDictionary<string, Journeys> MergedOdsJourneys { get; set; }      
    }
}
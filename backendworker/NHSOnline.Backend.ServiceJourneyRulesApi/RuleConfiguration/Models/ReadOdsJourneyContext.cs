using System.Collections.Generic;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class LoadContext
    {
        public FileData TargetSchema { get; set; }
        
        public IDictionary<string, Journeys> MergedOdsJourneys { get; set; }      
    }
}
using System.Collections.Generic;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class ConfigurationContext : LoadContext
    {
        
        public FileData RulesSchema { get; set; }
        
        public IDictionary<string, GpInfo> GpInfos { get; set; }
        
        public IDictionary<string, IEnumerable<TargetConfiguration>> FolderConfigurations { get; set; }
        
        public IDictionary<string, IDictionary<string, Journeys>> FolderOdsJourneys { get; set; }
        
    }
}
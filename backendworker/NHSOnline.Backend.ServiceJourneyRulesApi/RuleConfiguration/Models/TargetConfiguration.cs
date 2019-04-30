using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class TargetConfiguration
    {
        [YamlMember(Alias = "$schema")]
        public string Schema { get; set; }
        
        public Target Target { get; set; }
        
        public Journeys Journeys { get; set; }
    }
}
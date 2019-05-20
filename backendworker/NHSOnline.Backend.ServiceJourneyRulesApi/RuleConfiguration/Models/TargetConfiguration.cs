using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class TargetConfiguration
    {
        [YamlMember(Alias = "$schema", ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Schema { get; set; } = "Schemas/Journeys/configuration_schema.json";
        
        public Target Target { get; set; }
        
        public Journeys Journeys { get; set; }
    }
}
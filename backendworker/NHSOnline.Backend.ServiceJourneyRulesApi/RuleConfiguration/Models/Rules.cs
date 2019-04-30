using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class Rules
    {
        [YamlMember(Alias = "$schema")]
        public string Schema { get; set; }
        
        public List<string> FolderOrder { get; set; }
    }
}
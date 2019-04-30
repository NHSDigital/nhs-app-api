using NHSOnline.Backend.Support;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    public class Target
    {
        [YamlMember(Alias = Constants.Target.All)]
        public string All { get; set; }
        
        public string OdsCode { get; set; }
        
        public string CcgCode { get; set; }
        
        public Supplier Supplier { get; set; }
    }
}
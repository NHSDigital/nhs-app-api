using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Cdss : Journey<CdssProvider>, ICloneable<Cdss>
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string ServiceDefinition { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string ConditionsServiceDefinition { get; set; }

        public Cdss Clone() => MemberwiseClone() as Cdss;
    }
}
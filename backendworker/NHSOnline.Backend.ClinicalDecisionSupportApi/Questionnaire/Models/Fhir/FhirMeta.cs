using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirMeta
    {
        [JsonProperty(PropertyName = "versionId")]
        public string VersionId { get; set; }
        
        [JsonProperty(PropertyName = "lastUpdated")]
        public string LastUpdated { get; set; }
        
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
        
        [JsonProperty(PropertyName = "profile")]
        public string Profile { get; set; }
        
        [JsonProperty(PropertyName = "security")]
        public FhirCoding Security { get; set; }
        
        [JsonProperty(PropertyName = "tab")]
        public FhirCoding Tab { get; set; }
    }
}
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirContained
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "meta")]
        public FhirMeta FhirMeta { get; set; }
        
        [JsonProperty(PropertyName = "implicitRules")]
        public string ImplicitRules { get; set; }
        
        [JsonProperty(PropertyName = "language")]
        public FhirCoding Language { get; set; }
    }
}
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirQuantity
    {
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
        
        [JsonProperty(PropertyName = "comparator")]
        public string Comparator { get; set; }
        
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }
        
        [JsonProperty(PropertyName = "system")]
        public string System { get; set; }
        
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }
}
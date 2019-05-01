using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirOption
    {
        [JsonProperty(PropertyName = "valueInteger")]
        public int ValueInteger { get; set; }
        
        [JsonProperty(PropertyName = "valueDate")]
        public string ValueDate { get; set; }
        
        [JsonProperty(PropertyName = "valueTime")]
        public string ValueTime { get; set; }
        
        [JsonProperty(PropertyName = "valueString")]
        public string ValueString { get; set; }
        
        [JsonProperty(PropertyName = "valueCoding")]
        public FhirCoding ValueCoding { get; set; }
    }
}
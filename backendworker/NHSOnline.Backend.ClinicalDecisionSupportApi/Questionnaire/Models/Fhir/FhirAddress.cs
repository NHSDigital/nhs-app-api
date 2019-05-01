using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirAddress
    {
        [JsonProperty(PropertyName = "use")]
        public string Use { get; set; }
        
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        
        [JsonProperty(PropertyName = "line")]
        public string Line { get; set; }
        
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        
        [JsonProperty(PropertyName = "district")]
        public string District { get; set; }
        
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        
        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }
        
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
        
        [JsonProperty(PropertyName = "period")]
        public FhirPeriod FhirPeriod { get; set; }
    }
}
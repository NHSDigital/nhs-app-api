using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirText
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        [JsonProperty(PropertyName = "div")]
        public string Div { get; set; }
    }
}
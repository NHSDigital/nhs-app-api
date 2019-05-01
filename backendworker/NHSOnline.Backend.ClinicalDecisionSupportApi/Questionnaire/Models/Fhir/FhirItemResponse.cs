using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirItemResponse
    {
        [JsonProperty(PropertyName = "linkId")]
        public string LinkId { get; set; }
        
        [JsonProperty(PropertyName = "answer")]
        public FhirAnswer FhirAnswer { get; set; }
    }
}
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirPeriod
    {
        [JsonProperty(PropertyName = "start")]
        public string Start { get; set; }
        
        [JsonProperty(PropertyName = "end")]
        public string End { get; set; }
    }
}
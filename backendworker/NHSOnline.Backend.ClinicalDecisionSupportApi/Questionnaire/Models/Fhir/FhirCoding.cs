using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirCoding
    {
        [JsonProperty(PropertyName = "system")]
        public string System { get; set; }
        
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        
        [JsonProperty(PropertyName = "display")]
        public string Display { get; set; }

        [JsonProperty(PropertyName = "userSelected")]
        public bool UserSelected { get; set; }
    }
}
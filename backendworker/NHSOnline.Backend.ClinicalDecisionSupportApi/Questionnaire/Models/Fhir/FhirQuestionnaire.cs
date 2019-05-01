using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirQuestionnaire
    {
        public FhirQuestionnaire()
        {
            Item = new List<FhirItem>();
        }
        
        [JsonProperty(PropertyName = "resourceType")]
        public string ResourceType { get; set; }
        
        [JsonProperty(PropertyName = "text")]
        public FhirText Text { get; set; }

        [JsonProperty(PropertyName = "contained")]
        public FhirContained FhirContained { get; set; }
        
        [JsonProperty(PropertyName = "extension")]
        public FhirExtension FhirExtension { get; set; }
        
        [JsonProperty(PropertyName = "modifierExtension")]
        public FhirExtension ModifierFhirExtension { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        
        [JsonProperty(PropertyName = "subjectType")]
        public string SubjectType { get; set; }
        
        [JsonProperty(PropertyName = "item")]
        public List<FhirItem> Item { get; set; }
    }
}
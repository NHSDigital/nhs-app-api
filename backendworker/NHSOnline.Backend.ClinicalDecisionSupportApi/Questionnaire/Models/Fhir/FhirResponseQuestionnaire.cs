using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirResponseQuestionnaire
    {
        public FhirResponseQuestionnaire()
        {
            Item = new List<FhirItemResponse>();
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
        
        [JsonProperty(PropertyName = "questionnaire")]
        public string Questionnaire { get; set; }

        [JsonProperty(PropertyName = "item")]
        public List<FhirItemResponse> Item { get; set; }
    }
}
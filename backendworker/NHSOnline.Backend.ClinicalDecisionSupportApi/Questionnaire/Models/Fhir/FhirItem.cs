using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirItem
    {
        public FhirItem()
        {
            Item = new List<FhirItem>();
            Option = new List<FhirOption>();
        }
        
        [JsonProperty(PropertyName = "repeats")]
        public bool Repeats { get; set; }

        [JsonProperty(PropertyName = "item")]
        public List<FhirItem> Item { get; set; }

        [JsonProperty(PropertyName = "linkId")]
        public string LinkId { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "options")]
        public string Options { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "required")]
        public bool Required { get; set; }

        [JsonProperty(PropertyName = "option")]
        public List<FhirOption> Option { get; set; }
    }
}
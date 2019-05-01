using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirAnswer
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
        
        [JsonProperty(PropertyName = "valueBoolean")]
        public bool ValueBoolean { get; set; }
        
        [JsonProperty(PropertyName = "valueDecimal")]
        public double ValueDecimal { get; set; }
        
        [JsonProperty(PropertyName = "valueDateTime")]
        public string ValueDateTime { get; set; }
        
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "API returns a value of type string")]
        [JsonProperty(PropertyName = "valueUri")]
        public string ValueUri { get; set; }
        
        [JsonProperty(PropertyName = "valueAttachment")]
        public FhirAttachment ValueAttachment { get; set; }
        
        [JsonProperty(PropertyName = "valueQuantity")]
        public FhirQuantity ValueQuantity { get; set; }

        [JsonProperty(PropertyName = "valueReference")]
        public string ValueReference { get; set; }
    }
}
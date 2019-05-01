using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirExtension
    {
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "API returns a value of type string")]
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        
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
        public int ValueDecimal { get; set; }
        
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
        
        [JsonProperty(PropertyName = "valueBase64Binary")]
        public string ValueBase64Binary { get; set; }
        
        [JsonProperty(PropertyName = "valueCanonical")]
        public string ValueCanonical { get; set; }
        
        [JsonProperty(PropertyName = "valueCode")]
        public string ValueCode { get; set; }
        
        [JsonProperty(PropertyName = "valueId")]
        public string ValueId { get; set; }
        
        [JsonProperty(PropertyName = "valueInstant")]
        public string ValueInstant { get; set; }
        
        [JsonProperty(PropertyName = "valueMarkdown")]
        public string ValueMarkdown { get; set; }
        
        [JsonProperty(PropertyName = "valueOid")]
        public string ValueOid { get; set; }
        
        [JsonProperty(PropertyName = "valuePositiveInt")]
        public int ValuePositiveInt { get; set; }
        
        [JsonProperty(PropertyName = "valueUnsignedInt")]
        public int ValueUnsignedInt { get; set; }
        
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "API returns a value of type string")]
        [JsonProperty(PropertyName = "valueUrl")]
        public string ValueUrl { get; set; }
        
        [JsonProperty(PropertyName = "valueUuid")]
        public string ValueUuid { get; set; }
        
        [JsonProperty(PropertyName = "valueAddress")]
        public FhirAddress ValueAddress { get; set; }
    }
}
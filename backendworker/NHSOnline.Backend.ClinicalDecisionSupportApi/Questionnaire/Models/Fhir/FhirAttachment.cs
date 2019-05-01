using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir
{
    public class FhirAttachment
    {
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
        
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
        
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "API returns a value of type string")]
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "creation")]
        public string Creation { get; set; }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NHSOnline.HttpMocks.OnlineConsultations.Models
{
    public class OlcFhirRequest
    {
        [JsonPropertyName("resourceType")]
        public string? ResourceType { get; set; }

        [JsonPropertyName("parameter")]
        public IList<Parameter>? Parameter { get; set; } = new List<Parameter>();
    }

    public class Parameter
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("resource")]
        public Resource? Resource { get; set; }
    }

    public class Resource
    {
        [JsonPropertyName("resourceType")]
        public string? ResourceType { get; set; }
    }
}
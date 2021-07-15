using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NHSOnline.HttpMocks.OnlineConsultations.Models
{
    public class OlcFhirRequest
    {
        [JsonPropertyName("resourceType")]
        public string? ResourceType { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required to be serialized")]
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
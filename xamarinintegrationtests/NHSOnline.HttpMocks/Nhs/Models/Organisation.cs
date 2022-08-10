using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NHSOnline.HttpMocks.Nhs.Models
{
    public class Organisation
    {
        [JsonPropertyName("value")]
        public List<OrganisationItem>? Value { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class OrganisationItem
    {
        public string? OrganisationName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Postcode { get; set; }
        public string? ODSCode { get; set; }
        public List<Metric> Metrics { get; set; } = new List<Metric>();
        public string? OrganisationSubType { get; set; }
        public Geocode? Geocode { get; set; }

        [JsonPropertyName("URL")]
        public string? Url { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
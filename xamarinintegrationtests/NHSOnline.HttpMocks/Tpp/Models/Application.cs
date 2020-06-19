using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class Application
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlAttribute("providerId")]
        public string? ProviderId { get; set; }

        [XmlAttribute("deviceType")]
        public string? DeviceType { get; set; }
    }
}
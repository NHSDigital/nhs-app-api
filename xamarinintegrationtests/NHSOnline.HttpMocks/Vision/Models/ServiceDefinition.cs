using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "serviceDefinition", Namespace = "urn:vision")]
    public sealed class ServiceDefinition
    {
        [XmlElement(ElementName = "name", Namespace = "urn:vision")]
        [Required]
        public string? Name { get; set; }

        [XmlElement(ElementName = "version", Namespace = "urn:vision")]
        [Required]
        public string? Version { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "visionRequest", Namespace = "urn:vision")]
    public sealed class VisionRequest
    {
        [XmlElement(ElementName = "serviceDefinition", Namespace = "urn:vision")]
        [Required]
        public ServiceDefinition? ServiceDefinition { get; set; }

        [XmlElement(ElementName = "serviceHeader", Namespace = "urn:vision")]
        [Required]
        public ServiceHeader? ServiceHeader { get; set; }

        [XmlElement(ElementName = "serviceContent", Namespace = "urn:vision")]
        [Required]
        public ServiceContent? ServiceContent { get; set; }

        [XmlAttribute(AttributeName = "vision", Namespace = "http://www.w3.org/2000/xmlns/")]
        [Required]
        public string? Vision { get; set; }
    }
}
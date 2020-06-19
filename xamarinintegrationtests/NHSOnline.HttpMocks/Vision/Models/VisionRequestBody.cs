using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public sealed class VisionRequestBody
    {
        [XmlElement(ElementName = "visionRequest", Namespace = "urn:vision")]
        [Required]
        public VisionRequest? VisionRequest { get; set; }
    }
}
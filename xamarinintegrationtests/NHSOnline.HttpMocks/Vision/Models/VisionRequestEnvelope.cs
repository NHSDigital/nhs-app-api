using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public sealed class VisionRequestEnvelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [Required]
        public VisionRequestBody? Body { get; set; }
    }
}

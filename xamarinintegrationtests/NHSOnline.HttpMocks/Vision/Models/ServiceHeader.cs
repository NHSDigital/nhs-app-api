using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "serviceHeader", Namespace = "urn:vision")]
    public sealed class ServiceHeader
    {
        [XmlElement(ElementName = "credentials", Namespace = "urn:vision")]
        [Required]
        public Credentials? Credentials { get; set; }
    }
}
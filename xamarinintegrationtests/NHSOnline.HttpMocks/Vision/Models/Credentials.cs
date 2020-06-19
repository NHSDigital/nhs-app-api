using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "credentials", Namespace = "urn:vision")]
    public sealed class Credentials
    {
        [XmlElement(ElementName = "rosuAccountId", Namespace = "urn:vision")]
        [Required]
        public string? RosuAccountId { get; set; }

        [XmlElement(ElementName = "apiKey", Namespace = "urn:vision")]
        [Required]
        public string? ApiKey { get; set; }
    }
}
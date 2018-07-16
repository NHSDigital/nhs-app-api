using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "credentials", Namespace = "urn:vision")]
    public class Credentials
    {
        [XmlElement(ElementName = "rosuAccountId", Namespace = "urn:vision")]
        public string RosuAccountId { get; set; }

        [XmlElement(ElementName = "apiKey", Namespace = "urn:vision")]
        public string ApiKey { get; set; }
    }
}

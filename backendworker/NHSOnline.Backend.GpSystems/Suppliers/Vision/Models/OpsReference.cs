using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "opsReference", Namespace = "urn:vision")]
    public class OpsReference
    {
        [XmlElement(ElementName = "provider", Namespace = "urn:vision")]
        public string Provider { get; set; }

        [XmlElement(ElementName = "accountId", Namespace = "urn:vision")]
        public string AccountId { get; set; }
    }
}

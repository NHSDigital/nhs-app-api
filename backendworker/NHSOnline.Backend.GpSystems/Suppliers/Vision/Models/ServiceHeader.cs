using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceHeader", Namespace = "urn:vision")]
    public class ServiceHeader
    {
        [XmlElement(ElementName = "id", Namespace = "urn:vision")]
        public string Id { get; set; }

        [XmlElement(ElementName = "creationTime", Namespace = "urn:vision")]
        public string CreationTime { get; set; }

        [XmlElement(ElementName = "target", Namespace = "urn:vision")]
        public Target Target { get; set; }

        [XmlElement(ElementName = "credentials", Namespace = "urn:vision")]
        public Credentials Credentials { get; set; }

        [XmlElement(ElementName = "opsReference", Namespace = "urn:vision")]
        public OpsReference OpsReference { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}

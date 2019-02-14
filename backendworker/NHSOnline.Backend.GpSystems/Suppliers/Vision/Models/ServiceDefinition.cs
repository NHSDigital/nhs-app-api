using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceDefinition", Namespace = "urn:vision")]
    public class ServiceDefinition
    {
        [XmlElement(ElementName = "name", Namespace = "urn:vision")]
        public string Name { get; set; }

        [XmlElement(ElementName = "version", Namespace = "urn:vision")]
        public string Version { get; set; }
    }
}

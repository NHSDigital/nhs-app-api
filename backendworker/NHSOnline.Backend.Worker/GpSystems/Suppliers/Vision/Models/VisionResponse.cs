using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "visionResponse", Namespace = "urn:vision")]
    public class VisionResponse<T>
    {
        [XmlElement(ElementName = "serviceDefinition", Namespace = "urn:vision")]
        public ServiceDefinition ServiceDefinition { get; set; }

        [XmlElement(ElementName = "serviceHeader", Namespace = "urn:vision")]
        public ServiceHeaderResponse ServiceHeader { get; set; }

        [XmlElement(ElementName = "serviceContent", Namespace = "urn:vision")]
        public T ServiceContent { get; set; }

        [XmlAttribute(AttributeName = "vision", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vision { get; set; }
    }
}

using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceContent", Namespace = "urn:vision")]
    public class ServiceContentResponse<T>
    {
        [XmlElement(ElementName = "configuration", Namespace = "urn:vision")]
        public T Payload { get; set; }
    }
}
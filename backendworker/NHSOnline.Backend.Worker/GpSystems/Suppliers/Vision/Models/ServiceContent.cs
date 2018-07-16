using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceContent", Namespace = "urn:vision")]
    public class ServiceContent<T>
    {
        [XmlElement(ElementName = "vos", Namespace = "urn:vision")]
        public T ServiceContentBody { get; set; }
    }
}

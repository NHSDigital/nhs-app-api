using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class VisionResponseBody<T>
    {
        [XmlElement(ElementName = "visionResponse", Namespace = "urn:vision")]
        public VisionResponse<T> VisionResponse { get; set; }

        [XmlElement(ElementName = "Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Fault Fault { get; set; }
    }
}

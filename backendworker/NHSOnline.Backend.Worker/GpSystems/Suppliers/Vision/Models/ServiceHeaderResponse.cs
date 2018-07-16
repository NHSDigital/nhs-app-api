using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceHeader", Namespace = "urn:vision")]
    public class ServiceHeaderResponse
    {
        [XmlElement(ElementName = "outcome", Namespace = "urn:vision")]
        public Outcome Outcome { get; set; }
    }
}

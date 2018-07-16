using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "outcome", Namespace = "urn:vision")]
    public class Outcome
    {
        [XmlElement(ElementName = "successful", Namespace = "urn:vision")]
        public string Successful { get; set; }

        [XmlElement(ElementName = "error", Namespace = "urn:vision")]
        public OutcomeError Error { get; set; }
    }
}
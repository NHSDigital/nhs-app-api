using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "target", Namespace = "urn:vision")]
    public class Target
    {
        [XmlElement(ElementName = "nationalCode", Namespace = "urn:vision")]
        public string NationalCode { get; set; }
    }
}

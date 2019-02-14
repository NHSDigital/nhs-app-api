using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class VisionFault
    {
        [XmlElement(ElementName = "error", Namespace = "urn:vision")]
        public FaultError Error { get; set; }
    }
}
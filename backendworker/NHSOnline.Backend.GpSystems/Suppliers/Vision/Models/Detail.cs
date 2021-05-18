using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot("detail")]
    public class Detail
    {
        [XmlElement(ElementName = "visionFault", Namespace = "urn:vision")]
        public VisionFault VisionFault { get; set; }
    }
}
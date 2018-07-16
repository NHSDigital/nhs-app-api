using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class Detail
    {
        [XmlElement(ElementName = "visionFault", Namespace = "urn:vision")]
        public VisionFault VisionFault { get; set; }
    }
}
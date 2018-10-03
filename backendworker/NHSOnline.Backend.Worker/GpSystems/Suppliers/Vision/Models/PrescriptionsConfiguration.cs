using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PrescriptionsConfiguration
    {
        [XmlElement(ElementName = "repeat_enabled", Namespace = "urn:vision")]
        public bool RepeatEnabled { get; set; }
    }
}

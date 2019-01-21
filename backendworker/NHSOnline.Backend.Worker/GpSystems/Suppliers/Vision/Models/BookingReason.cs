using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class BookingReason
    {
        [XmlElement(ElementName = "add", Namespace = "urn:vision")]
        public bool Add { get; set; }

        [XmlElement(ElementName = "display", Namespace = "urn:vision")]
        public bool Display { get; set; }

        [XmlElement(ElementName = "edit", Namespace = "urn:vision")]
        public bool Edit { get; set; }
    }
}
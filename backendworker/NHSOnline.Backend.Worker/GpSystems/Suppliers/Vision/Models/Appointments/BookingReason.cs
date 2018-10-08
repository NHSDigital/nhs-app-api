using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class BookingReason
    {
        [XmlElement(ElementName = "add", Namespace = "urn:vision")]
        public bool add { get; set; }

        [XmlElement(ElementName = "display", Namespace = "urn:vision")]
        public bool display { get; set; }

        [XmlElement(ElementName = "edit", Namespace = "urn:vision")]
        public bool edit { get; set; }
    }
}
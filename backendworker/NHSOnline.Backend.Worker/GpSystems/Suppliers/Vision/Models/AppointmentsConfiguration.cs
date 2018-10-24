using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class AppointmentsConfiguration
    {
        [XmlElement(ElementName = "enabled", Namespace = "urn:vision")]
        public bool BookingEnabled { get; set; } = true;
    }
}
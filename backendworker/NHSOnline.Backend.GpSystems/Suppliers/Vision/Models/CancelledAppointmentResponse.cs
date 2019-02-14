using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class CancelledAppointmentResponse
    {
        [XmlElement(ElementName = "appointment", Namespace = "urn:vision")]
        public CancelledAppointment CancelledAppointment { get; set; }
    }
}
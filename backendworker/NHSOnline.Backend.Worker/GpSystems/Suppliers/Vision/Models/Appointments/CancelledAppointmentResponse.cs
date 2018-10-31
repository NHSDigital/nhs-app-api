using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class CancelledAppointmentResponse
    {
        [XmlElement(ElementName = "appointment", Namespace = "urn:vision")]
        public CancelledAppointment CancelledAppointment { get; set; }
    }
}
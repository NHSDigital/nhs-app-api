using System.Xml.Serialization;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class BookAppointmentRequest: AbstractVisionRequest
    {
        private BookAppointmentRequest(){}
        
        public BookAppointmentRequest(IVisionUserSession userSession, AppointmentBookRequest request)
        {
            PatientId = userSession.PatientId;
            SlotId = request.SlotId;
            Reason = request.BookingReason;
        }
        
        [XmlElement(ElementName = "slotId", Namespace = "urn:vision")]
        public string SlotId { get; set; }
        
        [XmlElement(ElementName  = "reason", Namespace = "urn:vision")]
        public string Reason { get; set; }
    }
}
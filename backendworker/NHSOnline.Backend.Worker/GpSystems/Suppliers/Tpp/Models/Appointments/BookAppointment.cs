using System.Xml.Serialization;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    public class BookAppointment : AbstractTppRequestModel
    {
        public BookAppointment() { }
        public BookAppointment(TppUserSession userSession, AppointmentBookRequest request)
        {
            PatientId = userSession.PatientId;
            SessionId = request.SlotId;
            StartDate = request.StartTime.Value.ToLocalTime().ToString(TppClient.TppDateTimeFormat);
            EndDate = request.EndTime.Value.ToLocalTime().ToString(TppClient.TppDateTimeFormat);
            Notes = request.BookingReason;
            UnitId = userSession.UnitId;
            OnlineUserId = userSession.OnlineUserId;
        }

        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("sessionId")]
        public string SessionId { get; set; }

        [XmlAttribute("startDate")]
        public string StartDate { get; set; }

        [XmlAttribute("endDate")]
        public string EndDate { get; set; }

        [XmlAttribute("notes")]
        public string Notes { get; set; }

        [XmlIgnore]
        public override string RequestType => "BookAppointment";
    }
}

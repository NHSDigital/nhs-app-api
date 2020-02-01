using System;
using System.Globalization;
using System.Xml.Serialization;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class BookAppointment : AbstractTppRequestModel
    {
        private const string TppDateTimeFormat = "yyy-MM-ddTHH:mm:ss+00:00";

        private BookAppointment()
        {
        }

        public BookAppointment(ITppUserSession userSession, AppointmentBookRequest request, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            PatientId = userSession.PatientId;
            SessionId = request.SlotId;
            StartDate = dateTimeOffsetProvider.ConvertToLocalTime(request.StartTime.Value).ToString(TppDateTimeFormat, CultureInfo.InvariantCulture);
            EndDate = dateTimeOffsetProvider.ConvertToLocalTime(request.EndTime.Value).ToString(TppDateTimeFormat, CultureInfo.InvariantCulture);
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

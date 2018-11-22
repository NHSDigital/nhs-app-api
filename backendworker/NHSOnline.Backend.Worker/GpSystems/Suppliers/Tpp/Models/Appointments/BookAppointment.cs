using System;
using System.Globalization;
using System.Xml.Serialization;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class BookAppointment : AbstractTppRequestModel
    {
        private BookAppointment()
        {
        }

        public BookAppointment(TppUserSession userSession, AppointmentBookRequest request, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            PatientId = userSession.PatientId;
            SessionId = request.SlotId;
            StartDate = dateTimeOffsetProvider.ConvertToLocalTime(request.StartTime.Value).ToString(TppClient.TppDateTimeFormat, CultureInfo.InvariantCulture);
            EndDate = dateTimeOffsetProvider.ConvertToLocalTime(request.EndTime.Value).ToString(TppClient.TppDateTimeFormat, CultureInfo.InvariantCulture);
            Notes = request.BookingReason;
            UnitId = userSession.OdsCode;
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

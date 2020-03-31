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
        private BookAppointment()
        {
        }

        public BookAppointment(TppRequestParameters tppRequestParameters, AppointmentBookRequest request, BookingDates bookingDates)
        {
            PatientId = tppRequestParameters.PatientId;
            SessionId = request.SlotId;
            StartDate = bookingDates.StartDate;
            EndDate = bookingDates.EndDate;
            Notes = request.BookingReason;
            UnitId = tppRequestParameters.OdsCode;
            OnlineUserId = tppRequestParameters.OnlineUserId;
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

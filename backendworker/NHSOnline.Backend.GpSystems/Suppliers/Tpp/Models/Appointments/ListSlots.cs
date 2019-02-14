using System;
using System.Globalization;
using System.Xml.Serialization;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ListSlots : AbstractTppRequestModel
    {
        private const string ExpectedDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'f'Z'";

        private ListSlots() { }

        public ListSlots(ITppUserSession userSession, AppointmentSlotsDateRange dateRange)
        {
            StartDate = dateRange.FromDate.DateTime.ToString(ExpectedDateFormat, CultureInfo.InvariantCulture);
            NumberOfDays = (dateRange.ToDate.AddDays(1) - dateRange.FromDate).Days;

            UnitId = userSession.UnitId;
            PatientId = userSession.PatientId;
            OnlineUserId = userSession.OnlineUserId;
        }

        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("startDate")]
        public string StartDate { get; set; }

        [XmlAttribute("numberOfDays")]
        public int NumberOfDays { get; set; }

        [XmlIgnore]
        public override string RequestType => "ListSlots";
    }
}

using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ViewAppointments : AbstractTppRequestModel
    {
        private ViewAppointments() { }

        public ViewAppointments(ITppUserSession tppUserSession, bool futureAppointments)
        {
            UnitId = tppUserSession.UnitId;
            PatientId = tppUserSession.PatientId;
            OnlineUserId = tppUserSession.OnlineUserId;
            FutureAppointments = futureAppointments ? "Y" : "N";
        }

        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("futureAppointments")]
        public string FutureAppointments { get; set; }

        [XmlIgnore]
        public override string RequestType => "ViewAppointments";
    }
}

using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ViewAppointments : AbstractTppRequestModel
    {
        private ViewAppointments() { }

        public ViewAppointments(TppUserSession tppUserSession)
        {
            UnitId = tppUserSession.UnitId;
            PatientId = tppUserSession.PatientId;
            OnlineUserId = tppUserSession.OnlineUserId;
            FutureAppointments = "Y";
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

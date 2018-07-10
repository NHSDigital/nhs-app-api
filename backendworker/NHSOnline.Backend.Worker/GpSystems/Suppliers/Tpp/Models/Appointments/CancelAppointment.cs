using System;
using System.Xml.Serialization;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class CancelAppointment : AbstractTppRequestModel
    {
        public CancelAppointment(TppUserSession userSession, AppointmentCancelRequest request)
        {
            PatientId = userSession.PatientId;
            OnlineUserId = userSession.OnlineUserId;
            UnitId = userSession.UnitId;
            
            ApptId = request.AppointmentId;
        }

        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlAttribute("apptId")]
        public string ApptId { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "CancelAppointment";
    }
}
using System;
using System.Xml.Serialization;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class CancelAppointment : AbstractTppRequestModel
    {
        private CancelAppointment()
        {
        }

        public CancelAppointment(TppRequestParameters tppRequestParameters, AppointmentCancelRequest request)
        {
            PatientId = tppRequestParameters.PatientId;
            OnlineUserId = tppRequestParameters.OnlineUserId;
            UnitId = tppRequestParameters.OdsCode;

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
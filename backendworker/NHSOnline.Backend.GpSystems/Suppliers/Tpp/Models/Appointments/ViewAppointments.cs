using System;
using System.Xml.Serialization;
using Org.BouncyCastle.Asn1.X509;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ViewAppointments : AbstractTppRequestModel
    {
        private ViewAppointments() { }

        public ViewAppointments(TppRequestParameters tppRequestParameters, AppointmentViewType viewType)
        {
            UnitId = tppRequestParameters.OdsCode;
            PatientId = tppRequestParameters.PatientId;
            OnlineUserId = tppRequestParameters.OnlineUserId;
            FutureAppointments = viewType == AppointmentViewType.Future ? "Y" : "N";
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

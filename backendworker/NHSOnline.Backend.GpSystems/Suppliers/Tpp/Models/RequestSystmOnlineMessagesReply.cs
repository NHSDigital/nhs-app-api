using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class RequestSystmOnlineMessagesReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("uuid")]
        public string UuId { get; set; }

        [XmlAttribute("BookAppointments")]
        public string BookAppointments { get; set; }

        [XmlAttribute("RequestMedicationConfirmation")]
        public string RequestMedicationConfirmation { get; set; }

        [XmlAttribute("Medication")]
        public string Medication { get; set; }
    }
}
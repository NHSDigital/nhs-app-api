using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ViewAppointmentsReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("uuid")]
        public string UuId { get; set; }

        [XmlElement("Appointment", typeof(Appointment))]
        public List<Appointment> Appointments { get; set; }
    }
}

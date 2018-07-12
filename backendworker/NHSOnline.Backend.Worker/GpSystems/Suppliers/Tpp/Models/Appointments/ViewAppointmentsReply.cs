using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
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

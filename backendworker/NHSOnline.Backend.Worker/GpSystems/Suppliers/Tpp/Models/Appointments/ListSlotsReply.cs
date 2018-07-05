using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class ListSlotsReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("bookableDays")]
        public int BookableDays { get; set; }

        [XmlElement("Session", typeof(Session))]
        public List<Session> Sessions { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}

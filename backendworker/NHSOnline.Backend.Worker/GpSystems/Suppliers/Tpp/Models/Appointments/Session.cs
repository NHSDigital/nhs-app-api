using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class Session
    {
        [XmlAttribute("sessionId")]
        public string SessionId { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("staffDetails")]
        public string StaffDetails { get; set; }

        [XmlAttribute("location")]
        public string Location { get; set; }

        [XmlElement("Slot", typeof(Slot))]
        public List<Slot> Slots { get; set; }
    }
}

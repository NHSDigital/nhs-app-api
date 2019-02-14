using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Deliberately matching the name specified by the GPSS")]
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

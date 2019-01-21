using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class BookedAppointments
    {
        [XmlArray(ElementName = "pastAppointments", Namespace = "urn:vision")]
        [XmlArrayItem(ElementName = "slot", Namespace = "urn:vision")]
        public List<PastSlot> PastAppointments { get; set; }

        [XmlElement(ElementName = "settings", Namespace = "urn:vision")]
        public SlotSettings Settings { get; set; }
        
        [XmlArray(ElementName = "slots", Namespace = "urn:vision")]
        [XmlArrayItem(ElementName = "slot", Namespace = "urn:vision")]
        public List<BookedSlot> Slots { get; set; }

        [XmlElement(ElementName = "references", Namespace = "urn:vision")]
        public References References { get; set; }
    }
}
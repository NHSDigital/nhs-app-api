using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class References
    {
        [XmlElement(ElementName = "owner", Namespace = "urn:vision")]
        public List<Owner> Owners { get; set; }

        [XmlElement(ElementName = "session", Namespace = "urn:vision")]
        public List<SlotSession> Sessions { get; set; }

        [XmlElement(ElementName = "location", Namespace = "urn:vision")]
        public List<Location> Locations { get; set; }

        [XmlElement(ElementName = "slotType", Namespace = "urn:vision")]
        public List<SlotType> SlotTypes { get; set; }
    }
}
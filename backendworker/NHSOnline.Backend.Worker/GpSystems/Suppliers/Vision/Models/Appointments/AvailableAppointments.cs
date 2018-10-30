using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class AvailableAppointments
    {
        [XmlArray(ElementName = "slots", Namespace = "urn:vision")]
        [XmlArrayItem(ElementName = "slot", Namespace = "urn:vision")]
        public List<FreeSlot> Slots { get; set; }

        [XmlElement(ElementName = "references", Namespace = "urn:vision")]
        public References References { get; set; }
    }
}
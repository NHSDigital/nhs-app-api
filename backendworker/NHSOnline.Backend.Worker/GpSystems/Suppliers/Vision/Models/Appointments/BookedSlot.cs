using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class BookedSlot : Slot
    {
        [XmlElement(ElementName = "duration", Namespace = "urn:vision")]
        public string Duration { get; set; }

        [XmlElement(ElementName = "session", Namespace = "urn:vision")]
        public string Session { get; set; } 
    }
}
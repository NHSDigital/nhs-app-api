using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class SlotSession
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "location")]
        public string Location { get; set; }

        [XmlElement(ElementName = "owner")]
        public string Owner { get; set; }
    }
}
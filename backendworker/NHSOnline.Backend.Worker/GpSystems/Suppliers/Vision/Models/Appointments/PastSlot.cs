using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class PastSlot
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        
        [XmlElement(ElementName = "dateTime", Namespace = "urn:vision")]
        public string DateTime { get; set; }

        [XmlElement(ElementName = "owner", Namespace = "urn:vision")]
        public string Owner { get; set; }

        [XmlElement(ElementName = "location", Namespace = "urn:vision")]
        public string Location { get; set; }

        [XmlElement(ElementName = "type", Namespace = "urn:vision")]
        public string Type { get; set; }
    }
}

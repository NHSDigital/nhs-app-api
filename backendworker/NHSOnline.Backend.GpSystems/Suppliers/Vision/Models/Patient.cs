using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class Patient
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses
{
    public class Repeat : IRepeat
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "drug", Namespace = "urn:vision")]
        public string Drug { get; set; }

        [XmlElement(ElementName = "dosage", Namespace = "urn:vision")]
        public string Dosage { get; set; }

        [XmlElement(ElementName = "quantity", Namespace = "urn:vision")]
        public string Quantity { get; set; }
    }

}
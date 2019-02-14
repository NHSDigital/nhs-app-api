using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlType(TypeName = "message", Namespace = "urn:vision")]
    public class AppointmentsMessage
    {
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
    }
}
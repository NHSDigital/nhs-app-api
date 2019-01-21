using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class Description
    {
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
    }
}
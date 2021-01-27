using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlType(TypeName = "message", Namespace = "urn:vision")]
    public sealed class VisionMessage
    {
        [XmlText]
        public string Text { get; set; } = "";
        [XmlAttribute(AttributeName = "language")]
        public string? Language { get; set; }
    }
}
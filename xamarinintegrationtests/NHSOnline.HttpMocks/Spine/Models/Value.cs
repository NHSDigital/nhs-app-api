using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Value
    {
        [XmlAttribute(AttributeName="value")]
        public string? value { get; set; }

        [XmlAttribute(AttributeName="codeSystem")]
        public string? CodeSystem { get; set; }

        [XmlAttribute(AttributeName="code")]
        public string? Code { get; set; }
    }
}
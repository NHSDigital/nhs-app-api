using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Code
    {
        [XmlAttribute(AttributeName="code")]
        public string? code { get; set; }

        [XmlAttribute(AttributeName="codeSystem")]
        public string? CodeSystem { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PertinentSerialChangeNumber
    {

        [XmlElement(ElementName="code", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Code? Code { get; set; }

        [XmlElement(ElementName="value", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Value? Value { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="moodCode")]
        public string? MoodCode { get; set; }
    }
}
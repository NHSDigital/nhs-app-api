using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class ControlActEvent
    {
        [XmlElement(ElementName="author1", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Author1? Author1 { get; set; }

        [XmlElement(ElementName="subject", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Subject? Subject { get; set; }

        [XmlElement(ElementName="queryAck", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public QueryAck? QueryAck { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="moodCode")]
        public string? MoodCode { get; set; }
    }
}
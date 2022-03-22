using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PDSResponse
    {
        [XmlElement(ElementName="pertinentInformation", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PertinentInformation? PertinentInformation { get; set; }

        [XmlElement(ElementName="subject", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Subject? Subject { get; set; }

        [XmlAttribute(AttributeName="xsi", Namespace="http://www.w3.org/2000/xmlns/")]
        public string? Xsi { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="moodCode")]
        public string? MoodCode { get; set; }
    }
}
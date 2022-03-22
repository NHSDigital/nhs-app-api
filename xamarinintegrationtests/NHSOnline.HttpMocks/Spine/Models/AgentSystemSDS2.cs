using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class AgentSystemSDS2
    {

        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode{ get; set; }

        [XmlAttribute(AttributeName="determinerCode")]
        public string? DeterminerCode { get; set; }
    }
}
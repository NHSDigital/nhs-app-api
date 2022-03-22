using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class AgentSystemSDS
    {
        [XmlElement(ElementName="agentSystemSDS", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public AgentSystemSDS2? agentSystemSDS { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
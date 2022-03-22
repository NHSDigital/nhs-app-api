using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Author1
    {
        [XmlElement(ElementName="AgentSystemSDS", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public AgentSystemSDS? AgentSystemSDS { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class AssignedEntity
    {
        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
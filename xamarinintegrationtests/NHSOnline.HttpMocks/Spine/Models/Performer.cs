using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Performer
    {
        [XmlElement(ElementName="assignedEntity", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public AssignedEntity? AssignedEntity { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
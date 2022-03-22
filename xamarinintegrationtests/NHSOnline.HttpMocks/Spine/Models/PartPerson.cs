using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PartPerson
    {
        [XmlElement(ElementName="name", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Name? Name { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="determinerCode")]
        public string? DeterminerCode { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class SubjectOf8
    {
        [XmlElement(ElementName="previousNhsContact", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PreviousNhsContact? PreviousNhsContact { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
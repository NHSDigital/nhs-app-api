using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class COCTMT000203UK02PartOfWhole
    {
        [XmlElement(ElementName="partPerson", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PartPerson? PartPerson { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
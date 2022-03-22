using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class COCTMT000201UK02PartOfWhole
    {
        [XmlElement(ElementName="addr", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Addr? Addr { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
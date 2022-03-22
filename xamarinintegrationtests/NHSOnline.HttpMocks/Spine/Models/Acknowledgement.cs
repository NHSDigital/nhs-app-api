using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Acknowledgement
    {
        [XmlElement(ElementName="messageRef", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public MessageRef? MessageRef { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
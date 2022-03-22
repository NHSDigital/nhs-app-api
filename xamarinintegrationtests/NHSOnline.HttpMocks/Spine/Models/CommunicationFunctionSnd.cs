using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class CommunicationFunctionSnd
    {
        [XmlElement(ElementName="device", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Device? Device { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PertinentInformation
    {

        [XmlElement(ElementName="pertinentSerialChangeNumber", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PertinentSerialChangeNumber? PertinentSerialChangeNumber { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
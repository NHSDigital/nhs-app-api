using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Subject
    {
        [XmlElement(ElementName="patientRole", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PatientRole? PatientRole { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }

        [XmlElement(ElementName="PDSResponse", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PDSResponse? PdsResponse { get; set; }
    }
}
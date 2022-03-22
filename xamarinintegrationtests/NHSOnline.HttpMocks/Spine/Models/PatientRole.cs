using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PatientRole
    {
        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id { get; set; }

        [XmlElement(ElementName="patientPerson", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PatientPerson? PatientPerson { get; set; }

        [XmlElement(ElementName="subjectOf8", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public SubjectOf8? SubjectOf8 { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
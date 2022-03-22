using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class SubjectOf
    {
        [XmlElement(ElementName="patientCareProvisionEvent", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PatientCareProvisionEvent? PatientCareProvisionEvent { get; set; }

        [XmlAttribute(AttributeName="typeCode")]
        public string? TypeCode { get; set; }
    }
}
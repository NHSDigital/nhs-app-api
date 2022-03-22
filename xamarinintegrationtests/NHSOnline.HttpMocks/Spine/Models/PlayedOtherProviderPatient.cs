using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PlayedOtherProviderPatient
    {
        [XmlElement(ElementName="subjectOf", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public SubjectOf? SubjectOf { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }
    }
}
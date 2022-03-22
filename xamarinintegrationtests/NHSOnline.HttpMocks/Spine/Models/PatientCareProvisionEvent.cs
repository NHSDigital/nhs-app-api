using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PatientCareProvisionEvent
    {
        [XmlElement(ElementName="code", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Code? Code { get; set; }

        [XmlElement(ElementName="effectiveTime", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public EffectiveTime? EffectiveTime { get; set; }

        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id{ get; set; }

        [XmlElement(ElementName="performer", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Performer? Performer { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="moodCode")]
        public string? MoodCode { get; set; }
    }
}
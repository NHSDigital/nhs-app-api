using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class PatientPerson
    {
        [XmlElement(ElementName="administrativeGenderCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public AdministrativeGenderCode? AdministrativeGenderCode { get; set; }

        [XmlElement(ElementName="birthTime", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public BirthTime? BirthTime { get; set; }

        [XmlElement(ElementName="playedOtherProviderPatient", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public PlayedOtherProviderPatient? PlayedOtherProviderPatient { get; set; }

        [XmlElement(ElementName="COCT_MT000201UK02.PartOfWhole", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public COCTMT000201UK02PartOfWhole? COCTMT000201UK02PartOfWhole { get; set; }

        [XmlElement(ElementName="COCT_MT000203UK02.PartOfWhole", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public COCTMT000203UK02PartOfWhole? COCTMT000203UK02PartOfWhole { get; set; }

        [XmlAttribute(AttributeName="classCode")]
        public string? ClassCode { get; set; }

        [XmlAttribute(AttributeName="determinerCode")]
        public string? DeterminerCode { get; set; }
    }
}
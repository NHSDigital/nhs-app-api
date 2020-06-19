using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class Person
    {
        [XmlAttribute("patientId")]
        public string? PatientId { get; set; }

        [XmlAttribute("dateOfBirth")]
        public string? DateOfBirth { get; set; }

        [XmlAttribute("gender")]
        public string? Gender { get; set; }

        public NationalId? NationalId { get; set; }

        public PersonName? PersonName { get; set; }

        public Address? Address { get; set; }
    }
}
using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class Person
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        
        [XmlAttribute("gender")]
        public string Gender { get; set; }

        public NationalId NationalId { get; set; }

        public PersonName PersonName { get; set; }
        
        public TppAddress Address { get; set; }
    }
}
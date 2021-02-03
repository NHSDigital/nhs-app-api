using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    internal sealed class Account
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string? PatientId { get; set; }

        [XmlElement(ElementName = "name", Namespace = "urn:vision")]
        public string? Name { get; set; }

        [XmlElement(ElementName = "patientNumber", Namespace = "urn:vision")]
        public List<PatientNumber> PatientNumbers { get; } = new List<PatientNumber>();
    }
}
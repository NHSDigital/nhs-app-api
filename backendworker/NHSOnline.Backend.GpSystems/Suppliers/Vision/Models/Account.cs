using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Deliberately matching the name specified by the GPSS")]
    public class Account
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }

        [XmlElement(ElementName = "name", Namespace = "urn:vision")]
        public string Name { get; set; }

        [XmlElement(ElementName = "patientNumber", Namespace = "urn:vision")]
        public List<PatientNumber> PatientNumbers { get; set; }
    }
}
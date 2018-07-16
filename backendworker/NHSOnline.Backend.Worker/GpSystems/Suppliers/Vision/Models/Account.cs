
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "account", Namespace = "urn:vision")]
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
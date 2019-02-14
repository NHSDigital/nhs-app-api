using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class Request
    {
        [XmlElement(ElementName = "date", Namespace = "urn:vision")]
        public DateTime Date { get; set; }

        [XmlElement(ElementName = "status", Namespace = "urn:vision")]
        public Status Status { get; set; }

        [XmlElement(ElementName = "repeat", Namespace = "urn:vision")]
        public List<GetPrescriptionRepeat> Repeats { get; set; } = new List<GetPrescriptionRepeat>();
    }
}

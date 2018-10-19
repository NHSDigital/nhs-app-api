using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class Clinical
    {
        [XmlAttribute(AttributeName = "eventdate")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "read_term")]
        public string ReadTerm { get; set; }
        [XmlAttribute(AttributeName = "read_code")]
        public string ReadCode { get; set; }
        [XmlAttribute(AttributeName = "read_term2")]
        public string ReadTerm2 { get; set; }
        [XmlAttribute(AttributeName = "read_code2")]
        public string ReadCode2 { get; set; }
        [XmlAttribute(AttributeName = "drug_term")]
        public string DrugTerm { get; set; }
        [XmlAttribute(AttributeName = "subgroup_code")]
        public string SubGroupCode { get; set; }
        [XmlAttribute(AttributeName = "last_prescribed_date")]
        public string LastPrescribedDate { get; set; }
        [XmlAttribute(AttributeName = "first_prescribed_date")]
        public string FirstPrescribedDate { get; set; }
        [XmlAttribute(AttributeName = "dosage")]
        public string Dosage { get; set; }
        [XmlAttribute(AttributeName = "quantity")]
        public string Quantity { get; set; }
        [XmlAttribute(AttributeName = "packsize")]
        public string PackSize { get; set; }
    }
}

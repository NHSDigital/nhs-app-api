using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class Clinical
    {
        [XmlAttribute(AttributeName = "eventdate")]
        public DateTime EventDate { get; set; }
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
    }
}

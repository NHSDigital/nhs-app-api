using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    [XmlType("Events")]
    public class Event
    {
        [XmlAttribute("date")]
        public string Date { get; set; }
        
        [XmlAttribute("doneBy")]
        public string DoneBy { get; set; }
        
        [XmlAttribute("location")]
        public string Location { get; set; }
        
        [XmlElement("Item")]
        public List<RequestPatientRecordItem> Items { get; set; }
    }
}
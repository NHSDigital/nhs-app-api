using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class TestResultsViewReplyItem
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        
        [XmlAttribute("description")]
        public string Description { get; set; }
        
        [XmlAttribute("date")]
        public string Date { get; set; }
        
        [XmlText]    
        public string Value { get; set; }
    }
}
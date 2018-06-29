using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Medication
    {
        [XmlAttribute("drugId")]
        public string DrugId { get; set; }
        
        [XmlAttribute("type")]
        public string Type { get; set; }
        
        [XmlAttribute("drug")]
        public string Drug { get; set; }
        
        [XmlAttribute("details")]
        public string Details { get; set; }
       
        [XmlAttribute("requestable")]
        public string Requestable { get; set; }
    }
}
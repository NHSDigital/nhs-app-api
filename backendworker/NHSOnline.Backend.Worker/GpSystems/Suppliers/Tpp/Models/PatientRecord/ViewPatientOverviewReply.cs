using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class ViewPatientOverviewReply
    {
       
        [XmlArray("DrugSensitivities")]
        [XmlArrayItem("Item", Type = typeof(Item))]
        public List<Item> DrugSensitivities { get; set; }  
        
        [XmlArray("Drugs")]
        [XmlArrayItem("Item", Type = typeof(Item))]
        public List<Item> Drugs { get; set; }  
        
        [XmlArray("PastRepeats")]
        [XmlArrayItem("Item", Type = typeof(Item))]
        public List<Item> PastRepeats { get; set; }  
        
        [XmlArray("CurrentRepeats")]
        [XmlArrayItem("Item", Type = typeof(Item))]
        public List<Item> CurrentRepeats { get; set; }  
        
        [XmlArray("Allergies")]
        [XmlArrayItem("Item", Type = typeof(Item))]
        public List<Item> Allergies { get; set; }  
    }
}
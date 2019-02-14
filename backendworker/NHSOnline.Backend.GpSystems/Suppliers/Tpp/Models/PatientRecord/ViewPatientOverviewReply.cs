using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class ViewPatientOverviewReply
    {
       
        [XmlArray("DrugSensitivities")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public List<ViewPatientOverViewItem> DrugSensitivities { get; set; }  
        
        [XmlArray("Drugs")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public List<ViewPatientOverViewItem> Drugs { get; set; }  
        
        [XmlArray("PastRepeats")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public List<ViewPatientOverViewItem> PastRepeats { get; set; }  
        
        [XmlArray("CurrentRepeats")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public List<ViewPatientOverViewItem> CurrentRepeats { get; set; }  
        
        [XmlArray("Allergies")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public List<ViewPatientOverViewItem> Allergies { get; set; }  
    }
}
using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class PatientSelected : AbstractTppRequestModel
    {        
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
    
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }        
    
        [XmlIgnore]
        public override string RequestType => "PatientSelected";
    }
}
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions
{
    [Serializable]
    public class ListRepeatMedicationReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlElement("Medication", typeof(Medication))]
        public List<Medication> Medications { get; set; }
        
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }       
    }
}
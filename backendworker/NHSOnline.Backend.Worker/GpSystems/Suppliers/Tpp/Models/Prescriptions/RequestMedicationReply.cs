using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions
{
    public class RequestMedicationReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlAttribute("message")]
        public string Message { get; set; }
        
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }       
    }
}
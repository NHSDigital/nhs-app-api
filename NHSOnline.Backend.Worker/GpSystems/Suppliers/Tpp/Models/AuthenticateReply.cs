using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class AuthenticateReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        public User User { get; set; }
        
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}
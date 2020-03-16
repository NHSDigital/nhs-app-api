using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    public class MessagesViewReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute(AttributeName="uuid")]
        public string Uuid { get; set; }

        [XmlElement("Message", Type = typeof(Message))]
        public List<Message> Messages { get; set; }
    }
}
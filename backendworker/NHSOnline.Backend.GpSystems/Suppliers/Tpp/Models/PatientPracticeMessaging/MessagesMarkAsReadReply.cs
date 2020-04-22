using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    [XmlRoot(ElementName = "MessageMarkAsReadReply")]
    public class MessagesMarkAsReadReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("uuid")]
        public string Uuid { get; set; }
    }
}

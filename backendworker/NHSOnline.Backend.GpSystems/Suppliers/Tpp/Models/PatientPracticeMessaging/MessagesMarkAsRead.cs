using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    [XmlRoot(ElementName = "MessageMarkAsRead")]
    public class MessagesMarkAsRead : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlElement("Message", Type = typeof(Message))]
        public List<Message> Messages { get; set; }

        [XmlIgnore]
        public override string RequestType => "MessageMarkAsRead";
    }
}

using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    [XmlRoot(ElementName = "Message")]
    public class Message
    {
        [XmlAttribute(AttributeName = "messageId")]
        public string MessageId { get; set; }
    }
}

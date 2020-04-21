using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    public class MessageCreateReply
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute(AttributeName="uuid")]
        public string Uuid { get; set; }
    }
}
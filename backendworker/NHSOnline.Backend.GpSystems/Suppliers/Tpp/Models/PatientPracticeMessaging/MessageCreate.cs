using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    public class MessageCreate: AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("recipientId")]
        public string RecipientId { get; set; }

        [XmlAttribute("unitRecipientId")]
        public string UnitRecipientId { get; set; }

        [XmlAttribute("message")]
        public string Message { get; set; }

        [XmlIgnore]
        public override string RequestType => "MessageCreate";
    }
}
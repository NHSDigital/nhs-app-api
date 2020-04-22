using System;
using System.Globalization;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    public class MessageDetails : Message
    {
        private bool? _hasSentDateTime;
        private DateTime? _sentDateTime;

        [XmlAttribute(AttributeName = "sent")]
        public string Sent { get; set; }

        public bool HasSentDateTime => (_hasSentDateTime = _hasSentDateTime ?? !string.IsNullOrWhiteSpace(Sent)).Value;

        public DateTime SentDateTime => (_sentDateTime = _sentDateTime ?? DateTime.Parse(Sent, CultureInfo.InvariantCulture)).Value;

        [XmlAttribute(AttributeName="messageText")]
        public string MessageText { get; set; }

        [XmlAttribute(AttributeName="conversationId")]
        public string ConversationId { get; set; }

        [XmlAttribute(AttributeName="sender")]
        public string Sender { get; set; }

        [XmlAttribute(AttributeName="recipient")]
        public string Recipient { get; set; }
        public YesNo Incoming { get; set; }
        public YesNo Read { get; set; }
        public YesNo Deleted { get; set; }

        [XmlAttribute(AttributeName="incoming")]
        public string IncomingString
        {
            set => Incoming = ConvertToEnum(value);
            get => Incoming.ToString();
        }

        [XmlAttribute(AttributeName="deleted")]
        public string DeletedString
        {
            set => Deleted = ConvertToEnum(value);
            get => Deleted.ToString();
        }

        [XmlAttribute(AttributeName="read")]
        public string ReadString
        {
            set => Read = ConvertToEnum(value);
            get => Read.ToString();
        }

        [XmlAttribute(AttributeName="binaryDataId")]
        public string BinaryDataId { get; set; }

        public MessageDetails()
        {
            Incoming = YesNo.n;
            Read = YesNo.n;
            Deleted = YesNo.n;
        }

        private static YesNo ConvertToEnum(string enumString)
        {
            return (YesNo) Enum.Parse(typeof(YesNo), enumString);
        }
    }
}
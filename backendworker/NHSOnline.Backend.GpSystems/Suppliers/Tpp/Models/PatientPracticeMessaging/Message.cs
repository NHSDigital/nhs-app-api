using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    [Serializable]
    [XmlRoot(ElementName="Message")]
    public class Message {
        [XmlAttribute(AttributeName="messageId")]
        public string MessageId { get; set; }

        [XmlAttribute(AttributeName="sent")]
        public string Sent { get; set; }

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

        public Message()
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
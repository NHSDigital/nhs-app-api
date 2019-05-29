using System.Xml.Serialization;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class UpdateNominatedPharmacyResponseEnvelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Header Header { get; set; }
    }

    [XmlRoot(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Header
    {
        [XmlElement(ElementName = "MessageHeader", Namespace = "http://www.oasis-open.org/committees/ebxml-msg/schema/msg-header-2_0.xsd")]
        public MessageHeader MessageHeader { get; set; }
    }

    [XmlRoot(ElementName = "MessageHeader", Namespace = "http://www.oasis-open.org/committees/ebxml-msg/schema/msg-header-2_0.xsd")]
    public class MessageHeader
    {
        [XmlElement(ElementName = "ConversationId", Namespace = "http://www.oasis-open.org/committees/ebxml-msg/schema/msg-header-2_0.xsd")]
        public string ConversationId { get; set; }
    }
}
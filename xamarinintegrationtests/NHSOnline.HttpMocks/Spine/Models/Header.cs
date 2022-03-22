using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Header
    {
        [XmlElement(ElementName="MessageID", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public string? MessageId { get; set; }

        [XmlElement(ElementName="Action", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public string? Action { get; set; }

        [XmlElement(ElementName="To", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public string? To { get; set; }

        [XmlElement(ElementName="From", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public From? From { get; set; }

        [XmlElement(ElementName="communicationFunctionRcv", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public CommunicationFunctionRcv? CommunicationFunctionRcv { get; set; }

        [XmlElement(ElementName="communicationFunctionSnd", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public CommunicationFunctionSnd? CommunicationFunctionSnd { get; set; }

        [XmlElement(ElementName="RelatesTo", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public string? RelatesTo { get; set; }
    }
}
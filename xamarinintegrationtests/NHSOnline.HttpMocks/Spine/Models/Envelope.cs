using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    [XmlRoot(ElementName="Envelope", Namespace="http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(ElementName="Header", Namespace="http://schemas.xmlsoap.org/soap/envelope/", IsNullable = true)]
        public Header? Header { get; set; }

        [XmlElement(ElementName="Body", Namespace="http://schemas.xmlsoap.org/soap/envelope/", IsNullable = true)]
        public Body? Body { get; set; }

        [XmlAttribute(AttributeName="crs", Namespace="http://www.w3.org/2000/xmlns/")]
        public string? Crs { get; set; }

        [XmlAttribute(AttributeName="SOAP-ENV", Namespace="http://www.w3.org/2000/xmlns/")]
        public string? SOAPENV { get; set; }

        [XmlAttribute(AttributeName="wsa", Namespace="http://www.w3.org/2000/xmlns/")]
        public string? Wsa { get; set; }

        [XmlAttribute(AttributeName="xmlns")]
        public string? Xmlns { get; set; }

        [XmlAttribute(AttributeName="hl7", Namespace="http://www.w3.org/2000/xmlns/")]
        public string? Hl7 { get; set; }
    }
}
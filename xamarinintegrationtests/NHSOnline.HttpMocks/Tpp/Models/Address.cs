using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class Address
    {
        [XmlAttribute(AttributeName = "address")]
        public string? AddressText { get; set; }

        [XmlAttribute(AttributeName = "addressType")]
        public string? AddressType { get; set; }
    }
}
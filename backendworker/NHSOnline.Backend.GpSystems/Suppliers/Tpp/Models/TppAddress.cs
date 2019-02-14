using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    public class TppAddress
    {
        [XmlAttribute(AttributeName="address")]
        public string Address { get; set; }
        [XmlAttribute(AttributeName="addressType")]
        public string AddressType { get; set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage
{
    [XmlRoot("LinkAccount")]
    public class LinkAccountCreate : LinkAccount
    {
        [XmlAttribute("nhsNumber")]
        public string NhsNumber { get; set; }

        [XmlAttribute("emailAddress")]
        public string EmailAddress { get; set; }

        [XmlAttribute("mobileNo")]
        public string MobileNumber { get; set; }
    }
}

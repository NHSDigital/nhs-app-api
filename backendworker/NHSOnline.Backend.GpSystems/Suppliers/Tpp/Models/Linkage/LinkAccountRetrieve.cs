using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage
{
    [XmlRoot("LinkAccount")]
    public class LinkAccountRetrieve : LinkAccount
    {
        [XmlAttribute("retrieveOnly")]
        public string RetrieveOnly { get; set; } = "y";

        [XmlAttribute("nhsNumber")]
        public string NhsNumber { get; set; }
    }
}

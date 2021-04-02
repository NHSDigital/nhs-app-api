using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage
{
    [XmlRoot("LinkAccount")]
    public class LinkAccountAuthenticate : LinkAccount
    {
        [XmlAttribute("accountId")]
        public string AccountId { get; set; }

        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }
    }
}

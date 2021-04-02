using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class LinkAccountReply
    {
        [XmlAttribute("accountId")]
        public string AccountId { get; set; }

        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        [XmlAttribute("passphraseToLink")]
        public string PassphraseToLink { get; set; }

        public string ProviderId { get; set; }
    }
}

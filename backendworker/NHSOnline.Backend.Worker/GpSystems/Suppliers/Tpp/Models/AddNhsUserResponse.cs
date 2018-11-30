using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [XmlType(TypeName = "LinkAccountReply")]
    public class AddNhsUserResponse
    {
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
        
        [XmlAttribute("accountId")]
        public string AccountId { get; set; }
        
        public string ProviderId { get; set; }

        [XmlAttribute("passphraseToLink")]
        public string PassphraseToLink { get; set; }
    }
}

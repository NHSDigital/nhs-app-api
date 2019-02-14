using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class LinkAccountReply
    {
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
        
        public string ProviderId { get; set; }
    }
}

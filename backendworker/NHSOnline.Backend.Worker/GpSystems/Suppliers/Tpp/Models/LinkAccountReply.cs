using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class LinkAccountReply
    {
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}

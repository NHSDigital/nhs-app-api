using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class Authenticate
    {
        [XmlAttribute("accountId")]
        public string? AccountId { get; set; }

        [XmlAttribute("passphrase")]
        public string? Passphrase { get; set; }

        [XmlAttribute("providerId")]
        public string? ProviderId { get; set; }

        public Application? Application { get; set; }
    }
}
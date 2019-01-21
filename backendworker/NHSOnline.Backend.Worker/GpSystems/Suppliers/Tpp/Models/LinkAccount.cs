using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class LinkAccount : ITppApplicationRequest
    {
        [XmlIgnore]
        public string RequestType => "LinkAccount";

        [XmlAttribute("apiVersion")]
        public string ApiVersion { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        [XmlAttribute("organisationCode")]
        public string OrganisationCode { get; set; }

        [XmlAttribute("accountId")]
        public string AccountId { get; set; }
        
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }

        [XmlAttribute("lastName")]
        public string LastName { get; set; }

        [XmlAttribute("dateOfBirth")]
        public DateTime DateofBirth { get; set; }

        public Application Application { get; set; }
    }
}

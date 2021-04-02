using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage
{
    public abstract class LinkAccount : ITppApplicationRequest
    {
        [XmlIgnore]
        public string RequestType => "LinkAccount";

        [XmlAttribute("apiVersion")]
        public string ApiVersion { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        [XmlAttribute("organisationCode")]
        public string OrganisationCode { get; set; }

        [XmlAttribute("lastName")]
        public string LastName { get; set; }

        [XmlAttribute("dateOfBirth")]
        public DateTime DateofBirth { get; set; }

        public Application Application { get; set; }
    }
}

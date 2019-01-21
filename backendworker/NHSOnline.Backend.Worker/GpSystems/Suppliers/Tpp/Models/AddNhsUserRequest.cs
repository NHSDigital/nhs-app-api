using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [XmlType(TypeName = "LinkAccount")]
    public class AddNhsUserRequest : ITppApplicationRequest
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
        
        [XmlAttribute("nhsNumber")]
        public string NhsNumber { get; set; }

        [XmlAttribute("dateOfBirth")]
        public DateTime DateofBirth { get; set; }

        public Application Application { get; set; }
    }
}

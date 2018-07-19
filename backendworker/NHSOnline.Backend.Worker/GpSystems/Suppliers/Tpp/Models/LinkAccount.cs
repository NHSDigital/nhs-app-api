using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class LinkAccount : ITppRequest
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

        public void ApplyConfig(ITppConfig tppConfig)
        {
            Application.Name = tppConfig.ApplicationName;
            Application.Version = tppConfig.ApplicationVersion;
            Application.ProviderId = tppConfig.ApplicationProviderId;
            Application.DeviceType = tppConfig.ApplicationDeviceType;
            ApiVersion = tppConfig.ApiVersion;
            Uuid = tppConfig.CreateGuid();
        }
    }
}

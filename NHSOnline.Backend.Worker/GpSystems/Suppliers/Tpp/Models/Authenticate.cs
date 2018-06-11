using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Authenticate
    {
        [XmlAttribute("apiVersion")]
        public string ApiVersion { get; set; }
        
        [XmlAttribute("accountId")]
        public string AccountId { get; set; }
        
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }
        
        [XmlAttribute("unitId")]
        public string UnitId { get; set; }
        
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        public Application Application { get; set; }
    }
}
using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Authenticate : AbstractTppRequestModel
    {        
        [XmlAttribute("accountId")]
        public string AccountId { get; set; }
        
        [XmlAttribute("passphrase")]
        public string Passphrase { get; set; }   
        
        [XmlAttribute("providerId")]
        public string ProviderId { get; set; }   

        public Application Application { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "Authenticate";
    }
}
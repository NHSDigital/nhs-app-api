using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "authenticationRef", Namespace = "urn:vision")]
    public class ServiceContentAuthenticationRef
    {       
        [XmlElement(ElementName = "apiToken", Namespace = "urn:vision")]
        public string ApiToken { get; set; }
    }
    
}

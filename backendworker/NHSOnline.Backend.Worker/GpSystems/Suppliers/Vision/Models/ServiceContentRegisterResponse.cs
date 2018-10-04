using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class ServiceContentRegisterResponse
    {
        [XmlElement(ElementName = "authenticationRef", Namespace = "urn:vision")]
        public ServiceContentAuthenticationRef AuthenticationRef { get; set; }
        
    }
}

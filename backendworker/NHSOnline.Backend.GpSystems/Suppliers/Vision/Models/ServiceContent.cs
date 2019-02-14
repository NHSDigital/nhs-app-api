using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "serviceContent", Namespace = "urn:vision")]
    public class ServiceContent<T>
    {
        [XmlElement(ElementName = "vos", Namespace = "urn:vision")]
        public T ServiceContentBody { get; set; }
        
        [XmlElement(ElementName = "register", Namespace = "urn:vision")]
        public ServiceContentRegister Register { get; set; }
        
        [XmlElement(ElementName = "authenticationRef", Namespace = "urn:vision")]
        public ServiceContentAuthenticationRef AuthenticationRef { get; set; }
        
        
    }
}

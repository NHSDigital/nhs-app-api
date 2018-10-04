using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "register", Namespace = "urn:vision")]
    public class ServiceContentRegister
    {
        [XmlElement(ElementName = "rosuAccountId", Namespace = "urn:vision")]
        public string RosuAccountId { get; set; }
        
        [XmlElement(ElementName = "rosuAccountLinkageKey", Namespace = "urn:vision")]
        public string RosuAccountLinkageKey { get; set; }
        
        [XmlElement(ElementName = "surname", Namespace = "urn:vision")]
        public string Surname { get; set; }
        
        [XmlElement(ElementName = "dob", Namespace = "urn:vision")]
        public string Dob { get; set; }
        
        [XmlElement(ElementName = "opsReference", Namespace = "urn:vision")]
        public OpsReference OpsReference { get; set; }
    }
}

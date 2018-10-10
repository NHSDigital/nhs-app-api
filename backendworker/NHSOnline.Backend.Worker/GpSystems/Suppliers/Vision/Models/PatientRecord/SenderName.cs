using System.Xml.Serialization;
namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlInclude(typeof(SenderName))]
    public class SenderName
    {
        [XmlElement(Namespace = "urn:vision")]
        public string UserName { get; set; }
        [XmlElement(Namespace = "urn:vision")]
        public string UserFullName { get; set; }
        [XmlElement(Namespace = "urn:vision")]
        public string UserIdentity { get; set; }
        [XmlElement(Namespace = "urn:vision")]
        public string UserRole{ get; set; }
    }
}

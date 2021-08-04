using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "vos", Namespace = "urn:vision")]
    public class Vos
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }
    }
}

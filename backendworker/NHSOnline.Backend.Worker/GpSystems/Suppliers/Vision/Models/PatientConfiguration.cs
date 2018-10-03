using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientConfiguration
    {
        [XmlElement(ElementName = "account", Namespace = "urn:vision")]
        public Account Account { get; set; }
        
        [XmlElement(ElementName = "prescriptions", Namespace = "urn:vision")]
        public PrescriptionsConfiguration Prescriptions { get; set; }
    }
}

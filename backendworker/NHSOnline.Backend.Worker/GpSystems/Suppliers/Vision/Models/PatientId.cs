using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientId
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string Id { get; set; }
    }
}

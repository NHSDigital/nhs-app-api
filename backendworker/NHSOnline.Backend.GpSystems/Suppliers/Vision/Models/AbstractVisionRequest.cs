using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class AbstractVisionRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }

    }
}
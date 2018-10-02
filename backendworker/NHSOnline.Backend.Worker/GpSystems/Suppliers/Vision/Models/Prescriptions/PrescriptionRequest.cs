using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class PrescriptionRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }
    }
}

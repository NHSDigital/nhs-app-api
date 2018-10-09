using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class DemographicsRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }
    }
}
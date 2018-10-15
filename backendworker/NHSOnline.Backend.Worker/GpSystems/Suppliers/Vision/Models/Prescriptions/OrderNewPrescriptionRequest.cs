using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class OrderNewPrescriptionRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }

        [XmlElement(ElementName = "repeat", Namespace = "urn:vision")]
        public List<NewPrescriptionRepeat> Repeats { get; set; } = new List<NewPrescriptionRepeat>();

        [XmlElement(ElementName = "message", Namespace = "urn:vision")]
        public string Message { get; set; }
    }
}

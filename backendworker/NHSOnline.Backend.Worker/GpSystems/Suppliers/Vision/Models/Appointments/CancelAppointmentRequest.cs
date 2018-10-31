using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class CancelAppointmentRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }
        
        [XmlElement(ElementName = "slotId", Namespace = "urn:vision")]
        public string SlotId { get; set; }
        
        [XmlElement(ElementName = "reasonId", Namespace = "urn:vision")]
        public string ReasonId { get; set; }
    }
}
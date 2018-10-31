using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class CancelledAppointment
    {
        [XmlElement(ElementName = "patient", Namespace = "urn:vision")]
        private Patient Patient { get;set; }
    }
}
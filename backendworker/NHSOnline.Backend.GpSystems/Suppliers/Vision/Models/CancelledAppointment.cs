using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class CancelledAppointment
    {
        [XmlElement(ElementName = "patient", Namespace = "urn:vision")]
        private Patient Patient { get;set; }
    }
}
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlType(TypeName = "appointment", Namespace = "urn:vision")]
    public class BookAppointmentResponse
    {       
        [XmlElement(ElementName = "slot", Namespace = "urn:vision")]
        public BookedSlot Slot { get; set; }
    }
}
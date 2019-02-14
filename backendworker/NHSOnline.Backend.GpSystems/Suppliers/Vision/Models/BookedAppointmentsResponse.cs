using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlType(TypeName = "bookedAppointmentsResponse", Namespace = "urn:vision")]
    public class BookedAppointmentsResponse
    {
        [XmlElement(ElementName = "bookedAppointmentsResponse", Namespace = "urn:vision")]
        public BookedAppointments Appointments { get; set; }
    }
}

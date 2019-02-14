using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlType(TypeName = "availableAppointmentsResponse", Namespace = "urn:vision")]
    public class AvailableAppointmentsResponse
    {
        [XmlElement(ElementName = "availableAppointmentsResponse", Namespace = "urn:vision")]
        public AvailableAppointments Appointments { get; set; }
    }
}

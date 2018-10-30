using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    [XmlType(TypeName = "availableAppointmentsResponse", Namespace = "urn:vision")]
    public class AvailableAppointmentsResponse
    {
        [XmlElement(ElementName = "availableAppointmentsResponse", Namespace = "urn:vision")]
        public AvailableAppointments Appointments { get; set; }
    }
}

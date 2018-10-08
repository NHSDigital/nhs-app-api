using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    [XmlType(TypeName = "bookedAppointmentsResponse", Namespace = "urn:vision")]
    public class BookedAppointmentsResponse
    {
        [XmlElement(ElementName = "bookedAppointmentsResponse", Namespace = "urn:vision")]
        public BookedAppointments Appointments { get; set; }
    }
}

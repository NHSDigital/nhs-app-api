using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentsGetResponse
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<SessionHolder> SessionHolders { get; set; }
        public IEnumerable<Session> Sessions { get; set; }
    }
}
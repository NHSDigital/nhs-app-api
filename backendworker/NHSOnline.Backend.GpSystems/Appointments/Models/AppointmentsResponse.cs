using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentsResponse
    {
        public IEnumerable<PastAppointment> PastAppointments { get; set; } = new List<PastAppointment>();
        public IEnumerable<UpcomingAppointment> UpcomingAppointments { get; set; } = new List<UpcomingAppointment>();
        public IEnumerable<CancellationReason> CancellationReasons { get; set; } = new List<CancellationReason>();

        public bool DisableCancellation { get; set; }
        public bool PastAppointmentsEnabled { get; set; }
    }
}

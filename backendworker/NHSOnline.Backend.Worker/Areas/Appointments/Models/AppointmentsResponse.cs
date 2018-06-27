using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentsResponse
    {
        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
        public IEnumerable<Clinician> Clinicians { get; set; } = new List<Clinician>();
        public IEnumerable<AppointmentSession> AppointmentSessions { get; set; } = new List<AppointmentSession>();
        public IEnumerable<Location> Locations { get; set; } = new List<Location>();
        public IEnumerable<CancellationReason> CancellationReasons { get; set; } = new List<CancellationReason>();
    }
}

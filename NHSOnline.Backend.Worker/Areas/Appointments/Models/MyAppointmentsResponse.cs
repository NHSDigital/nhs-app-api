using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class MyAppointmentsResponse
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public IEnumerable<Clinician> Clinicians { get; set; }
        public IEnumerable<AppointmentSession> AppointmentSessions { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<CancellationReason> CancellationReasons { get; set; }
    }
}

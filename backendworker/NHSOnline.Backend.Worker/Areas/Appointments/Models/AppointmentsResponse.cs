using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentsResponse
    {
        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
        public IEnumerable<CancellationReason> CancellationReasons { get; set; } = new List<CancellationReason>();

        public bool DisableCancellation { get; set; }
    }
}

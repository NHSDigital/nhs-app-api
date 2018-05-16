
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public IEnumerable<Clinician> Clinicians { get; set; } = new Clinician[0];
        public IEnumerable<AppointmentSession> AppointmentSessions { get; set; } = new AppointmentSession[0];
        public IEnumerable<Location> Locations { get; set; } = new Location[0];
        public IEnumerable<Slot> Slots { get; set; } = new Slot[0];
    }
}

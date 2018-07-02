using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public IEnumerable<Slot> Slots { get; set; } = new Slot[0];
    }
}

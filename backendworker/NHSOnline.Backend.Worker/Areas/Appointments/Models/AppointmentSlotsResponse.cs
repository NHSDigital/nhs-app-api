using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public string BookingGuidance { get; set; } = string.Empty;

        public IEnumerable<Slot> Slots { get; set; } = Array.Empty<Slot>();
    }
}

using System;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentBookRequest
    {
        public string SlotId { get; set; }
        
        public string BookingReason { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
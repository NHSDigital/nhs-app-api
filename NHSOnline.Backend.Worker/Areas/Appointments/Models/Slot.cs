using System;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class Slot
    {
        public string Id { get; set; }
        public DateTimeOffset StartTime { get; set; } 
        public DateTimeOffset? EndTime { get; set; }
        public string LocationId { get; set; }
        public string AppointmentSessionId { get; set; }
        public string[] ClinicianIds { get; set; }
        public string Type { get; set; }
    }
}

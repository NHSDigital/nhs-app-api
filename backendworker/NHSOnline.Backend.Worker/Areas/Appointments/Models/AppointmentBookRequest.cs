using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentBookRequest
    {
        [Required]
        public string SlotId { get; set; }

        [MaxLength(150)]
        public string BookingReason { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
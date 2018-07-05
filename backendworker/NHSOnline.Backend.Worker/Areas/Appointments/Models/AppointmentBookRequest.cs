using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentBookRequest
    {
        [Required]
        public string SlotId { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(1)]
        public string BookingReason { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
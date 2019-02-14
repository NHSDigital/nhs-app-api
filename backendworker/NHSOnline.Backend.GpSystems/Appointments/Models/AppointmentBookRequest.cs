using System;
using System.ComponentModel.DataAnnotations;
using NHSOnline.Backend.Support.ValidationAttributes;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentBookRequest
    {
        [Required]
        public string SlotId { get; set; }

        [SafeString]
        [MaxLength(150)]
        public string BookingReason { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
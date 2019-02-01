using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments.Models
{
    public class AppointmentCancelRequest
    {
        [Required]
        public string AppointmentId { get; set; }

        public string CancellationReasonId { get; set; }
    }
}
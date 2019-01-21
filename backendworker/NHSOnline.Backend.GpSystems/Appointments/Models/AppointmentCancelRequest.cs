namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentCancelRequest
    {
        public string AppointmentId { get; set; }

        public string CancellationReasonId { get; set; }
    }
}
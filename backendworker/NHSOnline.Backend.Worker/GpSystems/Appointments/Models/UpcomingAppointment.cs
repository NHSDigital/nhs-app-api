namespace NHSOnline.Backend.Worker.GpSystems.Appointments.Models
{
    public class UpcomingAppointment : Appointment
    {
        public bool DisableCancellation { get; set; }
    }
}

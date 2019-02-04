namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class UpcomingAppointment : Appointment
    {
        public bool DisableCancellation { get; set; }
    }
}
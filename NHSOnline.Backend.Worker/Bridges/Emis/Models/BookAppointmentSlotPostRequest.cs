namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class BookAppointmentSlotPostRequest
    {
        public string UserPatientLinkToken { get; set; }
        public long SlotId { get; set; }
        public string BookingReason { get; set; }
    }
}

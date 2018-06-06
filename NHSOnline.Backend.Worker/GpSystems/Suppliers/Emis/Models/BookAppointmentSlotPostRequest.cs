namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class BookAppointmentSlotPostRequest
    {
        public string UserPatientLinkToken { get; set; }
        public long SlotId { get; set; }
        public string BookingReason { get; set; }
    }
}

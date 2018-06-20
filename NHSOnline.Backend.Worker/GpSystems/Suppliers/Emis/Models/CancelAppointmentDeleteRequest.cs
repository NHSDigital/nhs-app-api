namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class CancelAppointmentDeleteRequest
    {
        public string UserPatientLinkToken { get; set; }
        public long SlotId { get; set; }
        public string CancellationReason { get; set; }
    }
}
namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class CancelAppointmentDeleteRequest
    {
        public CancelAppointmentDeleteRequest(string userPatientLinkToken, string cancellationReasonText, long slotId)
        {
            UserPatientLinkToken = userPatientLinkToken;
            CancellationReason = cancellationReasonText;
            SlotId = slotId;
        }

        public string UserPatientLinkToken { get; }
        public long SlotId { get; }
        public string CancellationReason { get; }
        
    }
}
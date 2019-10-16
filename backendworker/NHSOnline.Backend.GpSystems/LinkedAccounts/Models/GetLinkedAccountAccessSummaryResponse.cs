namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class GetLinkedAccountAccessSummaryResponse
    {
        public bool CanBookAppointment { get; set; }

        public bool CanOrderRepeatPrescription { get; set; }

        public bool CanViewMedicalRecord { get; set; }
    }
}

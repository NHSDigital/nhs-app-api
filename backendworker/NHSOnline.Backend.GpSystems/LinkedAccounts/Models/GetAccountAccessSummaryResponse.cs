namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class GetAccountAccessSummaryResponse
    {
        public bool IsValidData { get; set; }

        public bool CanBookAppointment { get; set; }

        public bool CanOrderRepeatPrescription { get; set; }

        public bool CanViewMedicalRecord { get; set; }
    }
}

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts.Models
{
    public class LinkedAccountAccessSummaryResponse
    {
        public string GpPracticeName { get; set; }

        public bool ShowSummary { get; set; }

        public bool CanBookAppointment { get; set; }

        public bool CanOrderRepeatPrescription { get; set; }

        public bool CanViewMedicalRecord { get; set; }
    }
}

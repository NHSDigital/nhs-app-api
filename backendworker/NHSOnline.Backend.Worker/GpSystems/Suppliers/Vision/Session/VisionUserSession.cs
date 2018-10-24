namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionUserSession : UserSession
    {
        public override Supplier Supplier => Supplier.Vision;

        public string RosuAccountId { get; set; }

        public string OdsCode { get; set; }

        public string PatientId { get; set; }

        public string ApiKey { get; set; }
        
        public bool IsRepeatPrescriptionsEnabled { get; set; }

        public bool IsAppointmentsEnabled { get; set; } = true;

        public bool AllowFreeTextPrescriptions { get; set; }
    }
}

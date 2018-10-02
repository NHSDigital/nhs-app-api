namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public static class PrescriptionRepeatStatusCode
    {
        public const int NotProcessed = -2;
        public const int InProgress = -1;
        public const int Rejected = 0;
        public const int Processed = 1;
    }
}

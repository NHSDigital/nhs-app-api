namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class PaycassoExternalReferences
    {
        public string ConsumerReference { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public bool HasNfcJourney { get; set; } = false;
    }
}
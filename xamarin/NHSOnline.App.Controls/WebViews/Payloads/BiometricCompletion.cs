namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class BiometricCompletion
    {
        public string Action { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
    }
}
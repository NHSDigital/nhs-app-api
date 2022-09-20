using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class SslErrorDetails
    {
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Url is set from a string")]
        public string? Url { get; set; } = string.Empty;

        public string? Error { get; set; } = string.Empty;
    }
}
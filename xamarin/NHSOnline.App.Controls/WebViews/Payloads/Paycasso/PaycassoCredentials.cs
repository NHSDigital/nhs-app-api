using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Controls.WebViews.Payloads.Paycasso
{
    public sealed class PaycassoCredentials
    {
        [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Matches published API")]
        public string HostUrl { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
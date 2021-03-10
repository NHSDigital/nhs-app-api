using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme { get; } = "http";
        public string Host { get; } = "web.local.bitraft.io";
        public int Port { get; } = 3000;
        
        [SuppressMessage("Performance", "CA1805:Do not initialize unnecessarily", Justification = "For clarity")]
        public bool NhsOnlineSessionCookieSecure { get; } = false;
    }
}
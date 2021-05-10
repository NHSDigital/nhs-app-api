using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "http";
        public string Host => "web.local.bitraft.io";
        public int Port => 3000;

        [SuppressMessage("Performance", "CA1805:Do not initialize unnecessarily", Justification = "For clarity")]
        public bool NhsOnlineSessionCookieSecure => false;
    }
}
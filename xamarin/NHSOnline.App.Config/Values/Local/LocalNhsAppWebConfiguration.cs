namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme { get; } = "http";
        public string Host { get; } = "web.local.bitraft.io";
        public int Port { get; } = 3000;
    }
}
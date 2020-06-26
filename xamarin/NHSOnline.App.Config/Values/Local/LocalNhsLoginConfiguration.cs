namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "http";
        public string AuthHost { get; } = "stubs.local.bitraft.io";
        public string UafHost { get; } = "stubs.local.bitraft.io";
        public int Port { get; } = 8080;
        public string AuthorizePath { get; } = "/citizenid/authorize";
    }
}
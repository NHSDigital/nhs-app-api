namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "http";
        public string BaseHost { get; } = "nhslogin.stubs.local.bitraft.io";
        public string AuthHost { get; } = "auth.nhslogin.stubs.local.bitraft.io";
        public string UafHost { get; } = "uaf.nhslogin.stubs.local.bitraft.io";
        public int Port { get; } = 8080;
        public string AuthorizePath { get; } = "/citizenid/authorize";
    }
}
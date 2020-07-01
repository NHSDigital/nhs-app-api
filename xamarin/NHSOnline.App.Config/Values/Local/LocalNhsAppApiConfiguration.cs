namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme { get; } = "http";
        public string Host { get; } = "api.local.bitraft.io";
        public int Port { get; } = 8089;
    }
}
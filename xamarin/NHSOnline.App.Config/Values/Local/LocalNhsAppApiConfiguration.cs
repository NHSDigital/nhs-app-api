namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "http";
        public string Host => "api.local.bitraft.io";
        public int Port => 8089;
    }
}
namespace NHSOnline.App.Config.Values.Production
{
    internal sealed class ProductionNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
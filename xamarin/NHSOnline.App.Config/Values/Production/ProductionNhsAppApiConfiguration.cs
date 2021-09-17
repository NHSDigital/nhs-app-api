namespace NHSOnline.App.Config.Values.Production
{
    internal sealed class ProductionNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
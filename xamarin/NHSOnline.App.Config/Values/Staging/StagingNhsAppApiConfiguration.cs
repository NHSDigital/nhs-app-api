namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-staging.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
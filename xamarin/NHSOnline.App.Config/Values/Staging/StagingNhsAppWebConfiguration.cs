namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-staging.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "www-staging.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
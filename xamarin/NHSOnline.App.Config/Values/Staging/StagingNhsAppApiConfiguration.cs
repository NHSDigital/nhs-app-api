namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "api-staging.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
namespace NHSOnline.App.Config.Values.Preview
{
    internal sealed class PreviewNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "www-preview.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
namespace NHSOnline.App.Config.Values.Preview
{
    internal sealed class PreviewNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "api-preview.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
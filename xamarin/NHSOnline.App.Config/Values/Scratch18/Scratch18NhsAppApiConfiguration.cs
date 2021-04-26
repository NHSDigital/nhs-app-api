namespace NHSOnline.App.Config.Values.Scratch18
{
    internal sealed class Scratch18NhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "api-scratch18.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
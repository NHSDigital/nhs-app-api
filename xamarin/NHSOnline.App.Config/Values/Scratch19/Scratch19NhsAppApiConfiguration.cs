namespace NHSOnline.App.Config.Values.Scratch19
{
    internal sealed class Scratch19NhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "api-scratch19.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
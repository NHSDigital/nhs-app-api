namespace NHSOnline.App.Config.Values.Scratch6
{
    internal sealed class Scratch6NhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme { get; } = "https";
        public string Host { get; } = "www-scratch6.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port { get; } = 443;
    }
}
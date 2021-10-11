namespace NHSOnline.App.Config.Values.Scratch
{
    internal sealed class ScratchNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        internal ScratchNhsAppWebConfiguration(string scratchEnvironment)
        {
            Host = $"www-{scratchEnvironment}.dev.nonlive.nhsapp.service.nhs.uk";
        }
        public string Scheme => "https";
        public string Host { get; }
        public int Port => 443;
    }
}
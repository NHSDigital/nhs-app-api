namespace NHSOnline.App.Config.Values.Scratch
{
    internal sealed class ScratchNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        internal ScratchNhsAppApiConfiguration(string scratchEnvironment)
        {
            Host = $"api-{scratchEnvironment}.dev.nonlive.nhsapp.service.nhs.uk";
        }
        public string Scheme => "https";
        public string Host { get; }
        public int Port => 443;
    }
}
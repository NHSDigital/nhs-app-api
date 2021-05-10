namespace NHSOnline.App.Config.Values.Scratch19
{
    internal sealed class Scratch19NhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-scratch19.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
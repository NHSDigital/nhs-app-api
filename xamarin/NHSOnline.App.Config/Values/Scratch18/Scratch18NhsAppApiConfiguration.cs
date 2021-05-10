namespace NHSOnline.App.Config.Values.Scratch18
{
    internal sealed class Scratch18NhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-scratch18.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
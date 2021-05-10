namespace NHSOnline.App.Config.Values.Scratch18
{
    internal sealed class Scratch18NhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-scratch18.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
namespace NHSOnline.App.Config.Values.Scratch6
{
    internal sealed class Scratch6NhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-scratch6.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
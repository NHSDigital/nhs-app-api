namespace NHSOnline.App.Config.Values.Preview
{
    internal sealed class PreviewNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-preview.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
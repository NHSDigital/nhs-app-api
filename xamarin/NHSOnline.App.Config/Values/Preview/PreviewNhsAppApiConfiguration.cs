namespace NHSOnline.App.Config.Values.Preview
{
    internal sealed class PreviewNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-preview.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
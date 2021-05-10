namespace NHSOnline.App.Config.Values.Scratch6
{
    internal sealed class Scratch6NhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-scratch6.dev.nonlive.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
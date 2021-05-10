namespace NHSOnline.App.Config.Values.OnboardingSandpit
{
    internal sealed class OnboardingSandpitNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-onboardingsandpit.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
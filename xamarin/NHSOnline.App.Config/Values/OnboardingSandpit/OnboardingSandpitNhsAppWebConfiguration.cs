namespace NHSOnline.App.Config.Values.OnboardingSandpit
{
    internal sealed class OnboardingSandpitNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-onboardingsandpit.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
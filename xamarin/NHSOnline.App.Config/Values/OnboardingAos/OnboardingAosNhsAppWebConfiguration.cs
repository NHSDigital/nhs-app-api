namespace NHSOnline.App.Config.Values.OnboardingAos
{
    internal sealed class OnboardingAosNhsAppWebConfiguration : INhsAppWebConfiguration
    {
        public string Scheme => "https";
        public string Host => "www-onboardingaos.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
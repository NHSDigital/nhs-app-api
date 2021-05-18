namespace NHSOnline.App.Config.Values.OnboardingAos
{
    internal sealed class OnboardingAosNhsAppApiConfiguration : INhsAppApiConfiguration
    {
        public string Scheme => "https";
        public string Host => "api-onboardingaos.nhsapp.service.nhs.uk";
        public int Port => 443;
    }
}
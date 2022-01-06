using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class FingerprintFaceIrisRegistrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FingerprintFaceIrisRegistrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Fingerprint, face or iris");

        private WebText YouCanLoginWithText => WebText.WithTagAndText(_interactor, "p",
            "You can log in with your fingerprint, face or iris instead of a password and security code if your device meets Google's increased security settings.");

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All fingerprints, faces or irises registered on this device will be able to access connected health websites and apps that use your NHS login information.");

        private WebToggle BiometricsToggle => WebToggle.WithLabel(_interactor, "Log in with fingerprint, face or iris");

        public void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            YouCanLoginWithText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
            BiometricsToggle.AssertToggledOff();
        }
    }
}
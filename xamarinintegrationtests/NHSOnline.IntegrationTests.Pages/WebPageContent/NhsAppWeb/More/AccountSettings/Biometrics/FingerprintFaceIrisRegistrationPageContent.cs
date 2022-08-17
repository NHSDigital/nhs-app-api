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

        private WebText IfYouShareThisDevicePanel => WebText.WithTagAndText(_interactor, "p",
            "Anyone else who uses fingerprint, face or iris recognition on this device can log in to your NHS app.");

        private WebText AdditionalInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "This means they can see your prescriptions and GP health record.");

        private WebToggle BiometricsToggle => WebToggle.WithLabel(_interactor, "Log in with fingerprint, face or iris");

        public void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            YouCanLoginWithText.AssertVisible();
            IfYouShareThisDevicePanel.AssertVisible();
            AdditionalInformationPanel.AssertVisible();
            BiometricsToggle.AssertToggledOff();
        }
    }
}
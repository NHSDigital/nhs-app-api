using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class TouchIdRegistrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TouchIdRegistrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Touch ID");

        private WebText TouchIdLetsYouLoginText => WebText.WithTagAndText(_interactor, "p",
            "Touch ID lets you log in with your fingerprint instead of a password and security code.");

        private WebText IfYouShareThisDevicePanel => WebText.WithTagAndText(_interactor, "p",
            "Anyone else who uses Touch ID on this device can log in to your NHS app.");

        private WebText AdditionalInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "This means they can see your prescriptions and GP health record.");

        private WebToggle BiometricsToggle => WebToggle.WithLabel(_interactor, "Log in with Touch ID");

        public void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            TouchIdLetsYouLoginText.AssertVisible();
            IfYouShareThisDevicePanel.AssertVisible();
            AdditionalInformationPanel.AssertVisible();
            BiometricsToggle.AssertToggledOff();
        }
    }
}
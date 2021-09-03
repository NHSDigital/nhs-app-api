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

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.");

        private WebToggle BiometricsToggle => WebToggle.WithLabel(_interactor, "Log in with Touch ID");

        public void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            TouchIdLetsYouLoginText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
            BiometricsToggle.AssertToggledOff();
        }
    }
}
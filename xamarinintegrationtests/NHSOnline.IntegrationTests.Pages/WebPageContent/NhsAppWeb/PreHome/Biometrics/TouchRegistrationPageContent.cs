using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class TouchRegistrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TouchRegistrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Turn on Touch ID");

        private WebText TouchLetsYouLoginText => WebText.WithTagAndText(_interactor, "p",
            "You can log in with your fingerprint Instead of a password and security code.");

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.");


        public void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TouchLetsYouLoginText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
        }
    }
}
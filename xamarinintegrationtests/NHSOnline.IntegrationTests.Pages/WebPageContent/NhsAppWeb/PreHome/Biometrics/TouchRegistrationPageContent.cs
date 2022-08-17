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
            "You can log in with your fingerprint instead of a password and security code.");

        private WebText IfYouShareThisDevicePanel => WebText.WithTagAndText(_interactor, "p",
            "Anyone else who uses Touch ID on this device can log in to your NHS app.");

        private WebText AdditionalInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "This means they can see your prescriptions and GP health record.");

        public void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TouchLetsYouLoginText.AssertVisible();
            IfYouShareThisDevicePanel.AssertVisible();
            AdditionalInformationPanel.AssertVisible();
        }
    }
}
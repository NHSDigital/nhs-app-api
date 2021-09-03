using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class FaceIdRegistrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FaceIdRegistrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Face ID");

        private WebText FaceIdLetsYouLoginText => WebText.WithTagAndText(_interactor, "p",
            "Face ID lets you log in with your face scan instead of a password and security code.");

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All Face IDs registered on this device will be able to access connected health websites and apps that use your NHS login information.");

        private WebToggle BiometricsToggle => WebToggle.WithLabel(_interactor, "Log in with Face ID");

        public void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            FaceIdLetsYouLoginText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
            BiometricsToggle.AssertToggledOff();
        }
    }
}
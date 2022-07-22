using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class FaceIdRegistrationPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FaceIdRegistrationPromptPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Turn on Face ID");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        private WebText FaceIdLetsYouLoginText => WebText.WithTagAndText(_interactor, "p",
            "You can log in with your face instead of a password and security code.");

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All faces registered on this device will be able to access connected health websites and apps that use your NHS login information.");

        private WebRadioOption NoTurnOffBiometricsOption => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Do you want to turn on Face ID?",
            "No, do not turn on Face ID");

        public void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            FaceIdLetsYouLoginText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
        }

        public void Continue() => ContinueButton.Click();

        public FaceIdRegistrationPromptPageContent NoTurnOffBiometrics()
        {
            NoTurnOffBiometricsOption.Click();
            return this;
        }
    }
}
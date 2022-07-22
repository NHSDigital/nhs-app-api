using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics
{
    public class FingerprintFaceIrisPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FingerprintFaceIrisPromptPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Turn on fingerprint, face or iris recognition");

        private WebText YouCanLoginWithText => WebText.WithTagAndText(_interactor, "p",
            "You can log in with your fingerprint, face or iris instead of a password and security code.");

        private WebText ImportantInformationPanel => WebText.WithTagAndText(_interactor, "p",
            "All fingerprints, faces or irises registered on this device will be able to access connected health websites and apps that use your NHS login information.");

        private WebRadioOption NoTurnOffBiometricsOption => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Do you want to turn on fingerprint, face or iris recognition?",
            "No, do not turn on fingerprint, face or iris recognition");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        public void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            YouCanLoginWithText.AssertVisible();
            ImportantInformationPanel.AssertVisible();
        }

        public FingerprintFaceIrisPromptPageContent NoTurnOffBiometrics()
        {
            NoTurnOffBiometricsOption.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();
    }
}
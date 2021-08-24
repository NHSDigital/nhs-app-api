using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class AdditionalGpServicesPrivacyPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdditionalGpServicesPrivacyPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "p",
            "We're about to ask you a few questions about your request. Your answers will be sent " +
            "securely to your GP surgery unless urgent medical attention is needed. For such cases " +
            "the online consultation service will direct you to other health services.");

        private WebCheckbox PrivacyConfirmationCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "I have read the GP online consultation services privacy notice. " +
            "I understand the online consultation service provider will process my personal " +
            "and health data on behalf of my GP surgery to provide an online consultation.");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public AdditionalGpServicesPrivacyPageContent AcceptPrivacyPolicy()
        {
            PrivacyConfirmationCheckbox.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();
    }
}
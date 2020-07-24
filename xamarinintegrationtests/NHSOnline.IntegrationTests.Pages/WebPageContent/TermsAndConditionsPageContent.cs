using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class TermsAndConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TermsAndConditionsPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Accept conditions of use");

        private WebCheckbox AcceptTermsAndConditionsCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "I understand and accept the terms of use and privacy policy. I accept the use of 'strictly necessary' cookies as detailed in the cookies policy.");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void AcceptTermsAndConditions()
        {
            AcceptTermsAndConditionsCheckbox.Click();
            ContinueButton.Click();
        }
    }
}

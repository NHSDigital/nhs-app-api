using System.Threading;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class TermsAndConditionsPage
    {
        private readonly IWebInteractor _interactor;

        private TermsAndConditionsPage(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => new WebText(_interactor, "h1", "Accept conditions of use");

        private WebCheckbox AcceptTermsAndConditionsCheckbox => new WebCheckbox(
            _interactor,
            "I understand and accept the terms of use and privacy policy. I accept the use of 'strictly necessary' cookies as detailed in the cookies policy.");

        private WebButton ContinueButton => new WebButton(_interactor, "Continue");

        internal static TermsAndConditionsPage AssertOnPage(IWebInteractor interactor)
        {
            var page = new TermsAndConditionsPage(interactor);
            page.Title.AssertVisible();
            return page;
        }

        internal TermsAndConditionsPage AcceptTermsAndConditions()
        {
            // Allow time for the Javascript to initialise
            // NHSO-10565: Remove once WebView is only displayed once fully loaded
            Thread.Sleep(1000);
            AcceptTermsAndConditionsCheckbox.Click();
            ContinueButton.Click();
            return this;
        }
    }
}

using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin
{
    public class StubbedLoginTermsAndConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginTermsAndConditionsPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS login - Terms and Conditions");

        private WebInputSubmit DeclineButton => WebInputSubmit.WithText(_interactor, "Decline");

        public void AssertOnPage() => TitleText.AssertVisible();

        public void DeclineTermsAndConditions() => DeclineButton.Click();
    }
}
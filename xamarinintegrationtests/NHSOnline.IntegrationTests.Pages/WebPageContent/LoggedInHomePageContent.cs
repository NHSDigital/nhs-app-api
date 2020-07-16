using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class LoggedInHomePageContent
    {
        private readonly IWebInteractor _interactor;

        internal LoggedInHomePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => new WebText(_interactor, "h1", "Home");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public LoggedInHomePageContent AssertWelcomeMessageDisplayedFor(string patientName)
        {
            new WebText(_interactor, "h2", $"Welcome, {patientName}").AssertVisible();
            return this;
        }
    }
}
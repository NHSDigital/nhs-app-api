using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class LoggedInHomePage
    {
        private readonly IWebInteractor _interactor;

        private LoggedInHomePage(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => new WebText(_interactor, "h1", "Home");

        internal static LoggedInHomePage AssertOnPage(IWebInteractor interactor)
        {
            var page = new LoggedInHomePage(interactor);
            page.Title.AssertVisible();
            return page;
        }

        public LoggedInHomePage AssertWelcomeMessageDisplayedFor(string patientName)
        {
            new WebText(_interactor, "h2", $"Welcome, {patientName}").AssertVisible();
            return this;
        }
    }
}
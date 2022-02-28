using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthUnavailablePageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthUnavailablePageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Sorry, your GP health record is unavailable");

        private WebText IfYouNeedThisText => WebText.WithTagAndText(_interactor,
            "p", "If you need this information now, contact your GP surgery.");

        private WebLink ReportAProblemLink => WebLink.WithText(_interactor, "Report a problem");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            IfYouNeedThisText.AssertVisible();
            ReportAProblemLink.AssertVisible();
        }

        public void ReportAProblem() => ReportAProblemLink.Click();
    }
}
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More
{
    public class LinkedProfilesUnavailablePageContent
    {
        private readonly IWebInteractor _interactor;

        internal LinkedProfilesUnavailablePageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot show linked profiles");

        private WebText IfYouNeedAccessText => WebText.WithTagAndText(_interactor,
            "p", "If you need to access health services on behalf of someone else, contact their GP surgery directly or try again later.");

        private WebLink ReportAProblemLink => WebLink.WithText(_interactor, "Report a problem");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            IfYouNeedAccessText.AssertVisible();
            ReportAProblemLink.AssertVisible();
        }

        public void ReportAProblem() => ReportAProblemLink.Click();
    }
}
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class ErsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ErsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "eRS");

        private WebLink InternalPageLink => WebLink.WithText(_interactor, "Internal Page");
        private WebLink NhsLoginLink => WebLink.WithText(_interactor, "NHS Login");
        private WebLink CovidLink => WebLink.WithText(_interactor, "Covid");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public ErsPageContent InternalPage()
        {
            InternalPageLink.Click();
            return this;
        }

        public ErsPageContent NhsLogin()
        {
            NhsLoginLink.Click();
            return this;
        }

        public ErsPageContent Covid()
        {
            CovidLink.Click();
            return this;
        }
    }
}
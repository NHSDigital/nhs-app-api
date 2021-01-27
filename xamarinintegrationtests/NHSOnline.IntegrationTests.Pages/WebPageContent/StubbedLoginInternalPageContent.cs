using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class StubbedLoginInternalPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginInternalPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS Login - Internal Page");

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void Back()
        {
            BackLink.Click();
        }
    }
}
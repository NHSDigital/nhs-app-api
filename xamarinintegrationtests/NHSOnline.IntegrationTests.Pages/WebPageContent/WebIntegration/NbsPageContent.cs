using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class NbsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NbsPageContent(IWebInteractor interactor) => _interactor = interactor;

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NBS Internal Page");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
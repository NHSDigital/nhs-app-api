using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class NdopPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NdopPageContent(IWebInteractor interactor)=>_interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Overview");
        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => BackBreadcrumb.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
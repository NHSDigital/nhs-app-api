using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class OrganDonationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OrganDonationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Something went wrong");

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
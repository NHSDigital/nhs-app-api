using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class OrganDonationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OrganDonationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Cannot access organ donation details");

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            TitleText.AssertVisible();
        }

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
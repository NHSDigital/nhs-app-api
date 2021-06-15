using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class AdditionalGpServicesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdditionalGpServicesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Additional GP services This service is provided by Engage Health Systems Limited");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
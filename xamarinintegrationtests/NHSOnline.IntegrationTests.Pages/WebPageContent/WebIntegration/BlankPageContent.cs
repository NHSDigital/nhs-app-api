using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class BlankPageContent
    {
        private readonly IWebInteractor _interactor;

        internal BlankPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => BackBreadcrumb.AssertVisible();
    }
}

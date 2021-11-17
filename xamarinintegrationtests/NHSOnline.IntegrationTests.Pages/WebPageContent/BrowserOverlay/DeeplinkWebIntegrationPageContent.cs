using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.BrowserOverlay
{
    public class DeeplinkWebIntegrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal DeeplinkWebIntegrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink DeepLink => WebLink.WithText(_interactor, "ERS Link");

        internal void AssertOnPage() => DeepLink.AssertVisible();
    }
}
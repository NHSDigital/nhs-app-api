using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class TestWebIntegrationProviderPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TestWebIntegrationProviderPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Internal Page");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }
    }
}
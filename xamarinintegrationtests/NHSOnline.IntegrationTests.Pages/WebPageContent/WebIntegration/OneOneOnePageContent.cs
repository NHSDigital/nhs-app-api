using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class OneOneOnePageContent
    {
        private readonly IWebInteractor _interactor;

        internal OneOneOnePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "111");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }
    }
}
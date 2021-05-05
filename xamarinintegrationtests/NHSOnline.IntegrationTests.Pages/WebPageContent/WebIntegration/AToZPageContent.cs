using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class AToZPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AToZPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Health A to Z");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }
    }
}
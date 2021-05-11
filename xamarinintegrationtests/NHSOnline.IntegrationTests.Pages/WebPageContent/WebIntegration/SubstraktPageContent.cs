using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class SubstraktPageContent
    {
        private readonly IWebInteractor _interactor;

        internal SubstraktPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Substrakt Internal Page");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }
    }
}
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class DeeplinkPageContent
    {
        private readonly IWebInteractor _interactor;

        internal DeeplinkPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink DeepLink => WebLink.WithText(_interactor, "Internal Appointments Link");

        internal void AssertOnPage() => DeepLink.AssertVisible();
    }
}
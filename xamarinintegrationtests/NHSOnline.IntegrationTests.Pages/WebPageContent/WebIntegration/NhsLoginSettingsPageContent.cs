using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class NhsLoginSettingsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NhsLoginSettingsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "NHS Login Settings");

        internal void AssertOnPage() => Title.AssertVisible();
    }
}
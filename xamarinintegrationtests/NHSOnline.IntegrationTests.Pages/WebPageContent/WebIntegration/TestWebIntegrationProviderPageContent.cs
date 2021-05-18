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

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Internal Page");

        private WebLink FileUploadLink => WebLink.WithText(_interactor, "File upload");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void NavigateToFileUpload() => FileUploadLink.Click();
    }
}
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class FileDownloadPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FileDownloadPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Document Download Page");

        private WebButton DownloadImageButton => WebButton.WithText(_interactor, "Download test image file");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void DownloadImage() => DownloadImageButton.Click();
    }
}
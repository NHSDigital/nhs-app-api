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

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Document Download");

        private WebButton DownloadImageButton => WebButton.WithText(_interactor, "Download test image file");

        private WebButton DownloadPassButton => WebButton.WithText(_interactor, "Download test pass kit file (ios only)");

        private WebButton DownloadCorruptedPassButton => WebButton.WithText(_interactor, "Download corrupted test pass kit file (ios only)");

        private WebButton DownloadCorruptedButton => WebButton.WithText(_interactor, "Download test corrupted file");

        private WebButton DownloadZipFileButton => WebButton.WithText(_interactor, "Download zip file");

        private WebButton DownloadInvalidNameFileButton => WebButton.WithText(_interactor, "Download test invalid name file");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void DownloadImage() => DownloadImageButton.Click();

        public void DownloadPass() => DownloadPassButton.Click();

        public void DownloadZip() => DownloadZipFileButton.Click();

        public void DownloadInvalidName() => DownloadInvalidNameFileButton.Click();

        public void DownloadCorruptedPass()
        {
            DownloadCorruptedPassButton.ScrollTo();
            DownloadCorruptedPassButton.Click();
        }

        public void DownloadCorrupted()
        {
            DownloadCorruptedButton.ScrollTo();
            DownloadCorruptedButton.Click();
        }
    }
}
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class FileUploadPageContent
    {
        private readonly IWebInteractor _interactor;

        internal FileUploadPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider File Upload Page");

        private WebFormLabel FileUploadLink => WebFormLabel.WithText(_interactor, "Upload a single file:");

        private WebText NotSelectedText => WebText.WithTagAndText(_interactor, "p", "No file selected");

        private WebText SelectedText => WebText.WithTagAndText(_interactor, "p", "File selected");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void UploadFile() => FileUploadLink.Click();

        public FileUploadPageContent AssertFileNotSelected()
        {
            NotSelectedText.AssertVisible();
            return this;
        }

        public void AssertFileSelected() => SelectedText.AssertVisible();
    }
}
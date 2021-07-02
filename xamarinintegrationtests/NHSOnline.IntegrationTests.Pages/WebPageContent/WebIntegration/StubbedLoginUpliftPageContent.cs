using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class StubbedLoginUpliftPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginUpliftPageContent(IWebInteractor webInteractor)
        {
            _interactor = webInteractor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS Login - Uplift");

        private WebFormLabel FileUploadButton => WebFormLabel.WithText(_interactor, "Open photo library");

        private WebFormLabel OpenCameraButton => WebFormLabel.WithText(_interactor, "Open Camera");

        private WebText FileNotSelected => WebText.WithTagAndText(_interactor, "p", "No file selected");

        private WebText FileSelected => WebText.WithTagAndText(_interactor, "p", "File selected");

        private WebText PhotoCaptured => WebText.WithTagAndText(_interactor, "p", "Photo captured");

        private WebText PhotoNotCaptured => WebText.WithTagAndText(_interactor, "p", "No photo captured");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void UploadFile() => FileUploadButton.Click();

        public void OpenCamera() => OpenCameraButton.Click();

        public void AssertNoFileSelected() => FileNotSelected.AssertVisible();

        public void AssertFileSelected() => FileSelected.AssertVisible();

        public void AssertPhotoCaptured() => PhotoCaptured.AssertVisible();

        public void AssertPhotoNotCaptured() => PhotoNotCaptured.AssertVisible();
    }
}
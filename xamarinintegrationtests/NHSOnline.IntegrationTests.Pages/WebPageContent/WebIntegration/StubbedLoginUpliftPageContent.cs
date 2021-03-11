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

        private WebLink PaycassoLink => WebLink.WithText(_interactor, "Paycasso");
        private WebFormLabel FileUploadButton => WebFormLabel.WithText(_interactor, "Open photo library");

        private WebText FileNotSelected => WebText.WithTagAndText(_interactor, "p", "No file selected");

        private WebText FileSelected => WebText.WithTagAndText(_interactor, "p", "File selected");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void UploadFile() => FileUploadButton.Click();

        public StubbedLoginUpliftPageContent AssertNoFileSelected()
        {
            FileNotSelected.AssertVisible();
            return this;
        }

        public void AssertFileSelected() => FileSelected.AssertVisible();

        public void Paycasso()
        {
            PaycassoLink.Click();
        }
    }
}
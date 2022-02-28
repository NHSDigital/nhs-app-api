using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class GpMedicalRecordPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpMedicalRecordPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your GP health record");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            TitleText.AssertVisible();
        }

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();

        public void Continue()
        {
            ContinueButton.ScrollTo();
            ContinueButton.Click();
        }
    }
}

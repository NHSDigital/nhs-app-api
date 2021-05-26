using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class GpMedicalRecordPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpMedicalRecordPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your GP health record");

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}

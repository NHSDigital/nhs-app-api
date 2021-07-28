using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class VaccineRecordPageContent
    {
        private readonly IWebInteractor _interactor;

        internal VaccineRecordPageContent(IWebInteractor interactor) => _interactor = interactor;

        public WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Vaccine Record Internal Page");

        internal void AssertOnPage() => Title.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
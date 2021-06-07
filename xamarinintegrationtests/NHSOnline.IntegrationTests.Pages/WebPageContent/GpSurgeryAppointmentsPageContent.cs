using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class GpSurgeryAppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpSurgeryAppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "GP Surgery Appointments");

        private WebText ErrorTitleText => WebText.WithTagAndText(_interactor, "h1", "Sorry, there is a problem with GP appointment booking");

        internal void AssertOnPage() => TitleText.AssertVisible();

        internal void AssertErrorOnPage() => ErrorTitleText.AssertVisible();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
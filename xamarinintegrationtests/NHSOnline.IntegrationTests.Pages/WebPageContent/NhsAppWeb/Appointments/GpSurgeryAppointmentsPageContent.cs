using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class GpSurgeryAppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpSurgeryAppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your GP appointments");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);
            
            TitleText.AssertVisible();
        }

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}
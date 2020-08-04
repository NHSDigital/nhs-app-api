using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class AppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Appointments");

        private WebMenuItem HospitalAndOtherAppointmentsMenuItem => WebMenuItem.WithTitle(_interactor, "Hospital and other appointments");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public AppointmentsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }

        public AppointmentsPageContent HospitalAndOtherAppointments()
        {
            HospitalAndOtherAppointmentsMenuItem.Click();
            return this;
        }
    }
}

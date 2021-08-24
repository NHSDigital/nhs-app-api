using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSAppointmentBookingUnavailablePage
    {
        private IOSFullNavigation Navigation { get; }
        private AppointmentBookingUnavailablePageContent PageContent { get; }

        private IOSAppointmentBookingUnavailablePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AppointmentBookingUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSAppointmentBookingUnavailablePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAppointmentBookingUnavailablePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}
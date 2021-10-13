using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidAppointmentBookingUnavailablePage
    {
        private AndroidFullNavigation Navigation { get; }
        public AppointmentBookingUnavailablePageContent PageContent { get; }

        private AndroidAppointmentBookingUnavailablePage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AppointmentBookingUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAppointmentBookingUnavailablePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAppointmentBookingUnavailablePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidAppointmentBookingUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
            return this;
        }
    }
}
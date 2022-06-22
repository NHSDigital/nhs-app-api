using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSAppointmentsPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSAppointmentsPage(IIOSDriverWrapper driver, bool isWayfinderEnabled)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AppointmentsPageContent(driver.Web.NhsAppLoggedInWebView(), isWayfinderEnabled);
        }

        public IOSFullNavigation Navigation { get; }

        public AppointmentsPageContent PageContent { get; }

        private IOSLink GpSurgeryAppointmentsLink => IOSLink.WithText(_driver,  "GP surgery appointments");

        public static IOSAppointmentsPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false, bool isWayfinderEnabled = false)
        {
            var page = new IOSAppointmentsPage(driver, isWayfinderEnabled);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSAppointmentsPage));
            }

            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
        }

        public void GoToGpSurgeryAppointments() => GpSurgeryAppointmentsLink.Touch();
    }
}

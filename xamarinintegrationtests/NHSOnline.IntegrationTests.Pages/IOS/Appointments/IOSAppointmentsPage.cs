using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSAppointmentsPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSAppointmentsPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public AppointmentsPageContent PageContent { get; }

        private IOSLink GpSurgeryAppointmentsLink => IOSLink.WithText(_driver,  "GP surgery appointments");

        public static IOSAppointmentsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }

        public void GoToGpSurgeryAppointments() => GpSurgeryAppointmentsLink.Touch();
    }
}

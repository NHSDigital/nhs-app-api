using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSAppointmentsPage
    {
        private IOSAppointmentsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        private IOSFullNavigation Navigation { get; }

        private AppointmentsPageContent PageContent { get; }

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
    }
}

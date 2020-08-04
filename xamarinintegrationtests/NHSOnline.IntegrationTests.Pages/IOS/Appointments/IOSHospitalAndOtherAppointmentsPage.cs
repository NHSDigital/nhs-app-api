using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSHospitalAndOtherAppointmentsPage
    {
        private IOSHospitalAndOtherAppointmentsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new HospitalAndOtherAppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        private IOSFullNavigation Navigation { get; }

        public HospitalAndOtherAppointmentsPageContent PageContent { get; }

        public static IOSHospitalAndOtherAppointmentsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSHospitalAndOtherAppointmentsPage(driver);
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
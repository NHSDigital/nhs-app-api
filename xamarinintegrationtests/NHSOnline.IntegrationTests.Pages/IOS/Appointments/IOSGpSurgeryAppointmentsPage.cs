using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSGpSurgeryAppointmentsPage
    {
        public GpSurgeryAppointmentsPageContent PageContent { get; }

        private IOSGpSurgeryAppointmentsPage(IIOSDriverWrapper driver)
        {
            PageContent = new GpSurgeryAppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSGpSurgeryAppointmentsPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSGpSurgeryAppointmentsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSGpSurgeryAppointmentsPage));
            }

            return page;
        }
    }
}
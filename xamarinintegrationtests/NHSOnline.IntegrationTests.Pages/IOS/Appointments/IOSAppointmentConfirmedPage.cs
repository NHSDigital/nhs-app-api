using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSAppointmentConfirmedPage
    {
        private ConfirmedAppointmentPageContent PageContent { get; }

        private IOSAppointmentConfirmedPage(IIOSDriverWrapper driver)
        {
            PageContent = new ConfirmedAppointmentPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSAppointmentConfirmedPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSAppointmentConfirmedPage));
            }
        }
    }
}
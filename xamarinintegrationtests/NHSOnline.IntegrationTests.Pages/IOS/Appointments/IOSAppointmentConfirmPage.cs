using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSAppointmentConfirmPage
    {
        public ConfirmAppointmentPageContent PageContent { get; }

        private readonly IIOSDriverWrapper _driver;

        private IOSAppointmentConfirmPage(IIOSDriverWrapper driver)
        {
            PageContent = new ConfirmAppointmentPageContent(driver.Web.NhsAppLoggedInWebView());
            _driver = driver;
        }

        public static IOSAppointmentConfirmPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSAppointmentConfirmPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSAppointmentConfirmedPage));
            }

            return page;
        }

        public void ScrollAndScreenshot()
        {
            PageContent.BackButton.ScrollTo();
            _driver.Screenshot($"{nameof(IOSAppointmentConfirmPage)}_scrolled");
        }
    }
}
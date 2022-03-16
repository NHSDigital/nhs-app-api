using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAppointmentConfirmPage
    {
        public ConfirmAppointmentPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAppointmentConfirmPage(IAndroidDriverWrapper driver)
        {
            PageContent = new ConfirmAppointmentPageContent(driver.Web.NhsAppLoggedInWebView());
            _driver = driver;
        }

        public static AndroidAppointmentConfirmPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidAppointmentConfirmPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidAppointmentConfirmPage));
            }

            return page;
        }

        public void ScrollAndScreenshot()
        {
            PageContent.BookingButton.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidAppointmentConfirmPage)}_scrolled");
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAppointmentConfirmedPage
    {
        private ConfirmedAppointmentPageContent PageContent { get; }

        private AndroidAppointmentConfirmedPage(IAndroidDriverWrapper driver)
        {
            PageContent = new ConfirmedAppointmentPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidAppointmentConfirmedPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidAppointmentConfirmedPage));
            }
        }
    }
}
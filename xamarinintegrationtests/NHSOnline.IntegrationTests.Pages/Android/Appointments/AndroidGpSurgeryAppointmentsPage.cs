using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidGpSurgeryAppointmentsPage
    {
        public GpSurgeryAppointmentsPageContent PageContent { get; }

        private AndroidGpSurgeryAppointmentsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new GpSurgeryAppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidGpSurgeryAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new AndroidGpSurgeryAppointmentsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidGpSurgeryAppointmentsPage));
            }

            return page;
        }
    }
}
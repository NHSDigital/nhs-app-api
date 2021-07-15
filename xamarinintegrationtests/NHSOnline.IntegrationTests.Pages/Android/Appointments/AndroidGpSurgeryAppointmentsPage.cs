using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
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

        public static AndroidGpSurgeryAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGpSurgeryAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public static AndroidGpSurgeryAppointmentsPage AssertErrorOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGpSurgeryAppointmentsPage(driver);
            page.PageContent.AssertErrorOnPage();
            return page;
        }
    }
}
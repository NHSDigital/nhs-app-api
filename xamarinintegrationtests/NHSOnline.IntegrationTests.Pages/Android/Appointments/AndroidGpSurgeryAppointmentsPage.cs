using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidGpSurgeryAppointmentsPage
    {
        public GpSurgeryAppointmentsPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidGpSurgeryAppointmentsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            PageContent = new GpSurgeryAppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
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
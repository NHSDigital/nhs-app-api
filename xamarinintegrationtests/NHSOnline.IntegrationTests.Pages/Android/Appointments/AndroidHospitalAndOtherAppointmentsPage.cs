using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidHospitalAndOtherAppointmentsPage
    {
        private AndroidHospitalAndOtherAppointmentsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new HospitalAndOtherAppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        private AndroidFullNavigation Navigation { get; }

        public HospitalAndOtherAppointmentsPageContent PageContent { get; }

        public static AndroidHospitalAndOtherAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidHospitalAndOtherAppointmentsPage(driver);
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
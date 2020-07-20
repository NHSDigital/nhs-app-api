using NHSOnline.IntegrationTests.Pages.Android.Settings;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidAppointmentsPage
    {
        private AndroidAppointmentsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        private AndroidFullNavigation Navigation { get; }

        private AppointmentsPageContent PageContent { get; }

        public static AndroidAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAppointmentsPage(driver);
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

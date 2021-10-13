using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidAppointmentsTemporaryProblemPage
    {
        private AndroidFullNavigation Navigation { get; }
        private AppointmentsTemporaryProblemPageContent PageContent { get; }

        private AndroidAppointmentsTemporaryProblemPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AppointmentsTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAppointmentsTemporaryProblemPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAppointmentsTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidAppointmentsTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
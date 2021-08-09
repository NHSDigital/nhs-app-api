using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public sealed class IOSAppointmentsTemporaryProblemPage
    {
        private IOSFullNavigation Navigation { get; }
        private AppointmentsTemporaryProblemPageContent PageContent { get; }

        private IOSAppointmentsTemporaryProblemPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AppointmentsTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSAppointmentsTemporaryProblemPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAppointmentsTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSAppointmentsTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSPrescriptionsTemporaryProblemPage
    {
        private IOSFullNavigation Navigation { get; }
        private PrescriptionsTemporaryProblemPageContent PageContent { get; }

        private IOSPrescriptionsTemporaryProblemPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PrescriptionsTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSPrescriptionsTemporaryProblemPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSPrescriptionsTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSPrescriptionsTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
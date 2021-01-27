using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSPrescriptionsPage
    {
        private IOSPrescriptionsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PrescriptionsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        private PrescriptionsPageContent PageContent { get; }

        public static IOSPrescriptionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSPrescriptionsPage(driver);
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

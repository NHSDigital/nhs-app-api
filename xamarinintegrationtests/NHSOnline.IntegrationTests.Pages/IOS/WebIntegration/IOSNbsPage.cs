using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSNbsPage
    {
        private IOSNbsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NbsPageContent(driver.Web.WebIntegrationWebView());
        }

        public IOSFullNavigation Navigation { get; }

        private NbsPageContent PageContent { get; }

        public static IOSNbsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNbsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertNativeHeader() => Navigation.AssertNavigationIconsArePresent();
    }
}
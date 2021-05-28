using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSPkbPage
    {
        private IOSPkbPage(IIOSDriverWrapper driver, string phrPath)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web(WebViewContext.PkbWebIntegration), phrPath);
        }

        public IOSFullNavigation Navigation { get; }

        private PkbPageContent PageContent { get; }

        public static IOSPkbPage AssertOnPage(IIOSDriverWrapper driver, string phrPath)
        {
            var page = new IOSPkbPage(driver, phrPath);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSOneOneOnePage
    {
        private IOSOneOneOnePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new OneOneOnePageContent(driver.Web.WebIntegrationWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public OneOneOnePageContent PageContent { get; }

        public static IOSOneOneOnePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSOneOneOnePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSOneOneOnePage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}
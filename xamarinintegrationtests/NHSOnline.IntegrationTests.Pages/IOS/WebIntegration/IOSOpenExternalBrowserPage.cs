using AngleSharp;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSOpenExternalBrowserPage
    {
        private IOSOpenExternalBrowserPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new OpenExternalBrowserPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public OpenExternalBrowserPageContent PageContent { get; }

        public static IOSOpenExternalBrowserPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSOpenExternalBrowserPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSOpenExternalBrowserPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public IOSOpenExternalBrowserPage ClickExtenalBrowserButton(IIOSDriverWrapper driver)
        {
            var page = new IOSOpenExternalBrowserPage(driver);
            page.PageContent.OpenExternalBrowser();
            return page;
        }

        public IOSOpenExternalBrowserPage EnterExternalURL(IIOSDriverWrapper driver, Url url)
        {
            var page = new IOSOpenExternalBrowserPage(driver);
            page.PageContent.EnterExternalUrl(url);
            return page;
        }
    }
}
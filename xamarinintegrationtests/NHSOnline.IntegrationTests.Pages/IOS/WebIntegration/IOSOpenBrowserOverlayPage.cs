using AngleSharp;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSOpenBrowserOverlayPage
    {
        private IOSOpenBrowserOverlayPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new OpenBrowserOverlayPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public OpenBrowserOverlayPageContent PageContent { get; }

        public static IOSOpenBrowserOverlayPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSOpenBrowserOverlayPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSOpenBrowserOverlayPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public IOSOpenBrowserOverlayPage clickOverlayButton(IIOSDriverWrapper driver)
        {
            var page = new IOSOpenBrowserOverlayPage(driver);
            page.PageContent.OpenBrowserOverlay();
            return page;
        }

        public IOSOpenBrowserOverlayPage EnterOverlayURL(IIOSDriverWrapper driver, Url url)
        {
            var page = new IOSOpenBrowserOverlayPage(driver);
            page.PageContent.EnterOverlayUrl(url);
            return page;
        }
    }
}
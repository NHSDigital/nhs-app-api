using NHSOnline.IntegrationTests.Pages.WebPageContent.BrowserOverlay;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidDeepLinkWebIntegrationLauncherPage
    {
        private AndroidDeepLinkWebIntegrationLauncherPage(IAndroidDriverWrapper driver)
        {
            PageContent = new DeeplinkWebIntegrationPageContent(driver.Web.BrowserOverlayWebView());
            _driver = driver;
        }

        public DeeplinkWebIntegrationPageContent PageContent { get; }

        private IAndroidDriverWrapper _driver;

        public static AndroidDeepLinkWebIntegrationLauncherPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidDeepLinkWebIntegrationLauncherPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        // A deep link requires a user interaction to function e.g. a touch, invoking click via JS will not work.
        public void ClickLink() => _driver.TouchScreenCentre();
    }
}
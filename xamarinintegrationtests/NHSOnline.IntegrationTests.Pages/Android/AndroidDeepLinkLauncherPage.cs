using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidDeepLinkLauncherPage
    {
        private AndroidDeepLinkLauncherPage(IAndroidDriverWrapper driver)
        {
            PageContent = new DeeplinkPageContent(driver.Web.BrowserOverlayWebView());
            _driver = driver;
        }

        public DeeplinkPageContent PageContent { get; }

        private IAndroidDriverWrapper _driver;

        public static AndroidDeepLinkLauncherPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidDeepLinkLauncherPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        // A deep link requires a user interaction to function e.g. a touch, invoking click via JS will not work.
        public void ClickLink() => _driver.TouchScreenCentre();
    }
}
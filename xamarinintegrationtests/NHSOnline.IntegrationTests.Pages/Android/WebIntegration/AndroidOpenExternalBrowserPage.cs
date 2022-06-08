using AngleSharp;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidOpenExternalBrowserPage
    {
        private AndroidOpenExternalBrowserPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new OpenExternalBrowserPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public OpenExternalBrowserPageContent PageContent { get; }

        public static AndroidOpenExternalBrowserPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOpenExternalBrowserPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidOpenExternalBrowserPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public AndroidOpenExternalBrowserPage ClickExternalBrowserButton(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOpenExternalBrowserPage(driver);
            page.PageContent.OpenExternalBrowser();
            return page;
        }

        public AndroidOpenExternalBrowserPage EnterExternalUrl(IAndroidDriverWrapper driver, Url url)
        {
            var page = new AndroidOpenExternalBrowserPage(driver);
            page.PageContent.EnterExternalUrl(url);
            return page;
        }
    }
}
using AngleSharp;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidOpenBrowserOverlayPage
    {
        private AndroidOpenBrowserOverlayPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new OpenBrowserOverlayPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public OpenBrowserOverlayPageContent PageContent { get; }

        public static AndroidOpenBrowserOverlayPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOpenBrowserOverlayPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidOpenBrowserOverlayPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public AndroidOpenBrowserOverlayPage clickOverlayButton(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOpenBrowserOverlayPage(driver);
            page.PageContent.OpenBrowserOverlay();
            return page;
        }

        public AndroidOpenBrowserOverlayPage EnterOverlayURL(IAndroidDriverWrapper driver, Url url)
        {
            var page = new AndroidOpenBrowserOverlayPage(driver);
            page.PageContent.EnterOverlayUrl(url);
            return page;
        }
    }
}
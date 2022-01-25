using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidFileDownloadPage
    {
        private AndroidFileDownloadPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new FileDownloadPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public FileDownloadPageContent PageContent { get; }

        public static AndroidFileDownloadPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidFileDownloadPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidFileDownloadPage AssertNativeHeaderAllIcons()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public AndroidFileDownloadPage AssertNativeHeaderHomeIcon()
        {
            Navigation.AssertHomeIconIsPresent();
            return this;
        }
    }
}
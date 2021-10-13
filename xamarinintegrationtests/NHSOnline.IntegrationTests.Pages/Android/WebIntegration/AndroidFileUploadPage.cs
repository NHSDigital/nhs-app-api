using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidFileUploadPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFileUploadPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new FileUploadPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public FileUploadPageContent PageContent { get; }

        public static AndroidFileUploadPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidFileUploadPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidFileUploadPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public AndroidFileUploadPage UploadTestFile()
        {
            _driver.PushTestFile();
            return this;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSFileUploadPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileUploadPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new FileUploadPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public FileUploadPageContent PageContent { get; }

        public static IOSFileUploadPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSFileUploadPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSFileUploadPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}
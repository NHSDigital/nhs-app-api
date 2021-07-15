using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSFileDownloadPage
    {
        private IOSFileDownloadPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new FileDownloadPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public FileDownloadPageContent PageContent { get; }

        public static IOSFileDownloadPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSFileDownloadPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSFileDownloadPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
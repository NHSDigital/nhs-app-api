using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSNdopWebIntegrationPage
    {
        private IOSNdopWebIntegrationPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NdopWebIntegrationPageContent(driver.Web.WebIntegrationWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public NdopWebIntegrationPageContent PageContent { get; }

        public static IOSNdopWebIntegrationPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNdopWebIntegrationPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
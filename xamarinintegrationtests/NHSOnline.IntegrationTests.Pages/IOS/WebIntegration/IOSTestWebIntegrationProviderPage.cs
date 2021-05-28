using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSTestWebIntegrationProviderPage
    {
        private IOSTestWebIntegrationProviderPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new TestWebIntegrationProviderPageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        public IOSFullNavigation Navigation { get; }

        public TestWebIntegrationProviderPageContent PageContent { get; }

        public static IOSTestWebIntegrationProviderPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSTestWebIntegrationProviderPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSTestWebIntegrationProviderPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSErsInternalPage
    {
        private IOSErsInternalPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new ErsInternalPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private IOSFullNavigation Navigation { get; }

        public ErsInternalPageContent PageContent { get; }

        public static IOSErsInternalPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSErsInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSErsInternalPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
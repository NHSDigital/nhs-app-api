using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSErsPage
    {
        private IOSErsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new ErsPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private IOSFullNavigation Navigation { get; }

        public ErsPageContent PageContent { get; }

        public static IOSErsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSErsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
        }
    }
}
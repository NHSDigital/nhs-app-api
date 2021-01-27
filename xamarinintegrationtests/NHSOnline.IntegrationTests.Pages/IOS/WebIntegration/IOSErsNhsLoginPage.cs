using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSErsNhsLoginPage
    {
        private IOSErsNhsLoginPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private IOSFullNavigation Navigation { get; }

        public StubbedLoginPageContent PageContent { get; }

        public static IOSErsNhsLoginPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSErsNhsLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSErsNhsLoginPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
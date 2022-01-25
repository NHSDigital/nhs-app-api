using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSGoToPage
    {
        private IOSFullNavigation Navigation { get; }
        public GoToPagePageContent PageContent { get; }

        private IOSGoToPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new GoToPagePageContent(driver.Web.WebIntegrationWebView());
        }

        public static IOSGoToPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSGoToPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSGoToPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public IOSGoToPage AssertNativeHeaderHome()
        {
            Navigation.AssertHomeIconIsPresent();
            return this;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More
{
    public sealed class IOSMorePage
    {
        private IOSMorePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new MorePageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        private MorePageContent PageContent { get; }

        public static IOSMorePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSMorePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}

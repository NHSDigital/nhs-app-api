using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth.Ndop
{
    public sealed class IOSNdopPage
    {
        private IOSNdopPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NdopPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public NdopPageContent PageContent { get; }

        public static IOSNdopPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNdopPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
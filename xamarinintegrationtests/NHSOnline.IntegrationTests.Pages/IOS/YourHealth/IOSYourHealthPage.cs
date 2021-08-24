using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public sealed class IOSYourHealthPage
    {
        private IOSYourHealthPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new YourHealthPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public YourHealthPageContent PageContent { get; }

        public static IOSYourHealthPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSYourHealthPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}

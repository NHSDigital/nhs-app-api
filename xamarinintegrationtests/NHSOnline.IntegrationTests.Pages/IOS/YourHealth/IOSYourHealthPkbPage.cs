using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public class IOSYourHealthPkbPage
    {
        private IOSYourHealthPkbPage(IIOSDriverWrapper driver)
            => PageContent = new YourHealthPkbViewPageContent(driver.Web.NhsAppLoggedInWebView());

        public YourHealthPkbViewPageContent PageContent { get; }

        public static IOSYourHealthPkbPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSYourHealthPkbPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
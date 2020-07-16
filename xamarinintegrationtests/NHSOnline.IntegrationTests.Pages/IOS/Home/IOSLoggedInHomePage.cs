using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSLoggedInHomePage
    {
        public LoggedInHomePageContent PageContent { get; }

        private IOSLoggedInHomePage(IIOSDriverWrapper driver)
        {
            PageContent = new LoggedInHomePageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static IOSLoggedInHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedInHomePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageDisplayedFor(string name)
        {
            PageContent.AssertWelcomeMessageDisplayedFor(name);
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSLoggedInHomePage
    {

        public IOSFullNavigation Navigation { get; }
        public LoggedInHomePageContent PageContent { get; }

        private IOSLoggedInHomePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
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
            Navigation.AssertNavigationPresent();
            PageContent.AssertWelcomeMessageDisplayedFor(name);
        }
    }
}
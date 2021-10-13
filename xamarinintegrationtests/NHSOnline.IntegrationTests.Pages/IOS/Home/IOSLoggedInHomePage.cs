using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Home;
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
            PageContent =
                new LoggedInHomePageContent(driver.Web.NhsAppLoggedInWebView(), LoggedInHomePageContent.BoiFaceId);
        }

        public static IOSLoggedInHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedInHomePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSLoggedInHomePage AssertPageDisplayedFor(string name)
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertNameDisplayedFor(name);
            return this;
        }

        public void ProveYourIdentityContinue() => PageContent.ProveYourIdentityContinue();
    }
}
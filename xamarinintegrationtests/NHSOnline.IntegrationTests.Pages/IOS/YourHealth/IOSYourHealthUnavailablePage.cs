using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public sealed class IOSYourHealthUnavailablePage
    {
        private IOSFullNavigation Navigation { get; }
        public YourHealthUnavailablePageContent PageContent { get; }

        private IOSYourHealthUnavailablePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new YourHealthUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSYourHealthUnavailablePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSYourHealthUnavailablePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSYourHealthUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}
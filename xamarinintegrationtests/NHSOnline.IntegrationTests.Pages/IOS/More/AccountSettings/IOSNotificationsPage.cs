using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings
{
    public class IOSNotificationsPage
    {
        private IOSNotificationsPage(IIOSDriverWrapper driver)
        {
            var webInteractor = driver.Web.NhsAppLoggedInWebView();

            Navigation = new IOSFullNavigation(driver);
            PageContent = new NotificationsPageContent(webInteractor);
            IOSPageContent = new IOSNotificationsPageContent(driver);
        }

        public IOSFullNavigation Navigation { get; }

        public NotificationsPageContent PageContent { get; }

        public IOSNotificationsPageContent IOSPageContent { get; }

        public static IOSNotificationsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNotificationsPage(driver);
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
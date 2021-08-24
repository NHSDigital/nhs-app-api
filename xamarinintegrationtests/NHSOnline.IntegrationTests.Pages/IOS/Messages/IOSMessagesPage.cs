using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Messages
{
    public sealed class IOSMessagesPage
    {
        private IOSMessagesPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new MessagesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public MessagesPageContent PageContent { get; }

        public static IOSMessagesPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSMessagesPage(driver);
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
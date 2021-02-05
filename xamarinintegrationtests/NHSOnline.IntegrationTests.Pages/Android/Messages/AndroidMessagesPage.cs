using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public sealed class AndroidMessagesPage
    {
        private AndroidMessagesPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new MessagesPageContent(driver.Web(WebViewContext.NhsApp));
        }

        private AndroidFullNavigation Navigation { get; }

        private MessagesPageContent PageContent { get; }

        public static AndroidMessagesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidMessagesPage(driver);
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

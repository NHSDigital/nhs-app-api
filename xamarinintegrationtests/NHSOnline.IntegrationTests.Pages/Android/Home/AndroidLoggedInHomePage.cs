using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidLoggedInHomePage
    {
        public LoggedInHomePageContent PageContent { get; }

        private AndroidLoggedInHomePage(IAndroidDriverWrapper driver)
        {
            PageContent = new LoggedInHomePageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidLoggedInHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedInHomePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageDisplayedFor(string name)
        {
            PageContent.AssertWelcomeMessageDisplayedFor(name);
        }
    }
}
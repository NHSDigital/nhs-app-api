using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More
{
    public sealed class AndroidNotificationsPage
    {
        private AndroidNotificationsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NotificationsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        public NotificationsPageContent PageContent { get; }

        public static AndroidNotificationsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNotificationsPage(driver);
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

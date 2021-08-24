using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public class AndroidNdopPage
    {
        private AndroidNdopPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NdopPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NdopPageContent PageContent { get; }

        public static AndroidNdopPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNdopPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
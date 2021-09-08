using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth.Ndop
{
    public sealed class AndroidNdopOverviewPage
    {
        private AndroidNdopOverviewPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NdopPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NdopPageContent PageContent { get; }

        public static AndroidNdopOverviewPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNdopOverviewPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
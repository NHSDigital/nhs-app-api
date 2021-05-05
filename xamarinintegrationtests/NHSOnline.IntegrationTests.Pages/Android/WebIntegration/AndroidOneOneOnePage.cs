using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidOneOneOnePage
    {
        private AndroidOneOneOnePage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new OneOneOnePageContent(driver.Web(WebViewContext.OneOneOneWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        private OneOneOnePageContent PageContent { get; }

        public static AndroidOneOneOnePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOneOneOnePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidOneOneOnePage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidErsInternalPage
    {
        private AndroidErsInternalPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new ErsInternalPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public ErsInternalPageContent PageContent { get; }

        public static AndroidErsInternalPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidErsInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidErsInternalPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
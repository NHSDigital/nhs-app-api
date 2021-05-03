using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidTestWebIntegrationProviderPage
    {
        private AndroidTestWebIntegrationProviderPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new TestWebIntegrationProviderPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public TestWebIntegrationProviderPageContent PageContent { get; }

        public static AndroidTestWebIntegrationProviderPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidTestWebIntegrationProviderPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidTestWebIntegrationProviderPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
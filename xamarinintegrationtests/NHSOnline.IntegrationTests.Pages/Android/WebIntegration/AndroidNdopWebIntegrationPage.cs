using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidNdopWebIntegrationPage
    {
        private AndroidNdopWebIntegrationPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NdopWebIntegrationPageContent(driver.Web.WebIntegrationWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NdopWebIntegrationPageContent PageContent { get; }

        public static AndroidNdopWebIntegrationPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNdopWebIntegrationPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidNdopWebIntegrationPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
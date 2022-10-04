using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidNbsPage
    {
        private AndroidNbsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NbsPageContent(driver.Web.WebIntegrationWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NbsPageContent PageContent { get; }

        public static AndroidNbsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            // Extending timeout to allow page to finish reloading
            using var extendedTimeout = ExtendedTimeout.FromSeconds(10);

            var page = new AndroidNbsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertNativeHeader() => Navigation.AssertNavigationIconsArePresent();
    }
}
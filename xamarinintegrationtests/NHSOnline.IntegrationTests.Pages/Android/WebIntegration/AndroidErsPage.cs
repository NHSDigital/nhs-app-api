using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidErsPage
    {
        private AndroidErsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new ErsPageContent(driver.Web(WebViewContext.ErsWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public ErsPageContent PageContent { get; }

        public static AndroidErsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidErsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
        }
    }
}
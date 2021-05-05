using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidAToZPage
    {
        private AndroidAToZPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AToZPageContent(driver.Web(WebViewContext.AToZWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        private AToZPageContent PageContent { get; }

        public static AndroidAToZPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAToZPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidAToZPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
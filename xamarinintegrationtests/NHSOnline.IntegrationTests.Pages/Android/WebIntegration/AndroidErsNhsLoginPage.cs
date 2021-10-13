using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidErsNhsLoginPage
    {
        private AndroidErsNhsLoginPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public StubbedLoginPageContent PageContent { get; }

        public static AndroidErsNhsLoginPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidErsNhsLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidErsNhsLoginPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}
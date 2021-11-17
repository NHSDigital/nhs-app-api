using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidStubbedLoginPageFullHeader
    {
        private AndroidStubbedLoginPageFullHeader(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginPageContent PageContent { get; }

        public AndroidFullNavigation Navigation { get; }

        public static AndroidStubbedLoginPageFullHeader AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginPageFullHeader(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidStubbedLoginPageFullHeader AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}
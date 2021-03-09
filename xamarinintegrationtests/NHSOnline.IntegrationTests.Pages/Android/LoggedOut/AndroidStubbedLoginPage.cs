using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidStubbedLoginPage
    {
        private AndroidStubbedLoginPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public StubbedLoginPageContent PageContent { get; }

        public AndroidSlimCloseNavigation Navigation { get; }

        public static AndroidStubbedLoginPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidStubbedLoginPageSlimHeader
    {
        private AndroidStubbedLoginPageSlimHeader(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginPageContent PageContent { get; }

        public AndroidSlimCloseNavigation Navigation { get; }

        public static AndroidStubbedLoginPageSlimHeader AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new AndroidStubbedLoginPageSlimHeader(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidStubbedLoginPageSlimHeader));
            }

            return page;
        }
    }
}
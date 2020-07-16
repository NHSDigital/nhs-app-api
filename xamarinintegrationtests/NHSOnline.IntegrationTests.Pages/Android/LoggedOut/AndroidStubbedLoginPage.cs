using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidStubbedLoginPage
    {
        public StubbedLoginPageContent PageContent { get; }

        private AndroidStubbedLoginPage(IAndroidDriverWrapper driver)
        {
            PageContent = new StubbedLoginPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public static AndroidStubbedLoginPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
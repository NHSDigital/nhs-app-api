using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidStubbedLoginInternalPage
    {
        private AndroidStubbedLoginInternalPage(IAndroidDriverWrapper driver)
        {
            PageContent = new StubbedLoginInternalPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public StubbedLoginInternalPageContent PageContent { get; }

        public static AndroidStubbedLoginInternalPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
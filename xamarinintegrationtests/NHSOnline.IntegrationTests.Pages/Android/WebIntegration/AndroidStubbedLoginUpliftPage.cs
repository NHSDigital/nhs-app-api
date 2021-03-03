using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidStubbedLoginUpliftPage
    {
        private AndroidStubbedLoginUpliftPage(IAndroidDriverWrapper driver)
        {
            PageContent = new StubbedLoginUpliftPageContent(driver.Web(WebViewContext.OneOff));
        }

        public StubbedLoginUpliftPageContent PageContent { get; }

        public static AndroidStubbedLoginUpliftPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginUpliftPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
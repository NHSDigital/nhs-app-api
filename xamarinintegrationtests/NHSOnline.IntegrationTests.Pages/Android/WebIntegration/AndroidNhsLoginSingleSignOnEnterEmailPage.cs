using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidNhsLoginSingleSignOnEnterEmailPage
    {
        private AndroidNhsLoginSingleSignOnEnterEmailPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            PageContent = new StubbedLoginEnterEmailPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginEnterEmailPageContent PageContent { get; }

        public AndroidSlimCloseNavigation Navigation { get; }

        public static AndroidNhsLoginSingleSignOnEnterEmailPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsLoginSingleSignOnEnterEmailPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
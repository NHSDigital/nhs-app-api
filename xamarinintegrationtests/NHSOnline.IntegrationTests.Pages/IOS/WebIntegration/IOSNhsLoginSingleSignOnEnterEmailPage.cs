using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSNhsLoginSingleSignOnEnterEmailPage
    {
        private IOSNhsLoginSingleSignOnEnterEmailPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSSlimCloseNavigation(driver);
            PageContent = new StubbedLoginEnterEmailPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginEnterEmailPageContent PageContent { get; }

        public IOSSlimCloseNavigation Navigation { get; }

        public static IOSNhsLoginSingleSignOnEnterEmailPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNhsLoginSingleSignOnEnterEmailPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSStubbedLoginPage
    {
        private IOSStubbedLoginPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSSlimCloseNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public StubbedLoginPageContent PageContent { get; }

        public IOSSlimCloseNavigation Navigation { get; }

        public static IOSStubbedLoginPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
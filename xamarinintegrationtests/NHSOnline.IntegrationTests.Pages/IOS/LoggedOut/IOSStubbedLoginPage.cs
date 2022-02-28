using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSStubbedLoginPage
    {
        private IOSStubbedLoginPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSSlimCloseNavigation(driver);
            PageContent = new StubbedLoginPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginPageContent PageContent { get; }

        public IOSSlimCloseNavigation Navigation { get; }

        public static IOSStubbedLoginPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSStubbedLoginPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSStubbedLoginPage));
            }

            return page;
        }
    }
}
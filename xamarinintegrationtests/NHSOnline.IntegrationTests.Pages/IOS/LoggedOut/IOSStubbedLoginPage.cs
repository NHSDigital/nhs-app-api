using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSStubbedLoginPage
    {
        private readonly IIOSDriverWrapper _driver;
        public StubbedLoginPageContent PageContent { get; }

        private IOSStubbedLoginPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            PageContent = new StubbedLoginPageContent(_driver.Web(WebViewContext.NhsLogin));
        }

        public static IOSStubbedLoginPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
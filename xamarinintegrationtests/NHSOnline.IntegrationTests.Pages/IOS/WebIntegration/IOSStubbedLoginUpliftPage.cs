using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSStubbedLoginUpliftPage
    {
        private IOSStubbedLoginUpliftPage(IIOSDriverWrapper driver)
        {
            PageContent = new StubbedLoginUpliftPageContent(driver.Web(WebViewContext.OneOff));
        }

        public StubbedLoginUpliftPageContent PageContent { get; }

        public static IOSStubbedLoginUpliftPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginUpliftPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
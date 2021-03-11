using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSStubbedLoginPaycassoPage
    {
        private IOSStubbedLoginPaycassoPage(IIOSDriverWrapper driver)
        {
            PageContent = new StubbedLoginPaycassoPageContent(driver.Web(WebViewContext.NhsLoginUplift));
        }
        
        public StubbedLoginPaycassoPageContent PageContent { get; }

        public static IOSStubbedLoginPaycassoPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginPaycassoPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
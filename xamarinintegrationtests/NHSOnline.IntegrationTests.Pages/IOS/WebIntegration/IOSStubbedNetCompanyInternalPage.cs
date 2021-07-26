using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSStubbedNetCompanyInternalPage
    {
        private IOSStubbedNetCompanyInternalPage(IIOSDriverWrapper driver)
            => PageContent = new NetCompanyInternalPageContent(driver.Web.WebIntegrationWebView());

        public NetCompanyInternalPageContent PageContent { get; }

        public static IOSStubbedNetCompanyInternalPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedNetCompanyInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
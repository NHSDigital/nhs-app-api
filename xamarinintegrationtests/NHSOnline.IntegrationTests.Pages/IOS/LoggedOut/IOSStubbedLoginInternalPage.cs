using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSStubbedLoginInternalPage
    {
        private IOSStubbedLoginInternalPage(IIOSDriverWrapper driver)
        {
            PageContent = new StubbedLoginInternalPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public StubbedLoginInternalPageContent PageContent { get; }

        public static IOSStubbedLoginInternalPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
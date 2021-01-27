using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.MyRecord
{
    public sealed class IOSMyRecordPage
    {
        private IOSMyRecordPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new MyRecordPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        private MyRecordPageContent PageContent { get; }

        public static IOSMyRecordPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSMyRecordPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}

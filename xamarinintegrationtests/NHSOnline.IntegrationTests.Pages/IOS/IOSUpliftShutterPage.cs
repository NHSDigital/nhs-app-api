using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSUpliftShutterPage
    {
        public IOSFullNavigation Navigation { get; }
        public UpliftShutterPageContent PageContent { get; }

        private IOSUpliftShutterPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new UpliftShutterPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static IOSUpliftShutterPage Continue(IIOSDriverWrapper driver)
        {
            var page = new IOSUpliftShutterPage(driver);
            page.PageContent.ProveYourIdentityContinue();
            return page;
        }
    }
}
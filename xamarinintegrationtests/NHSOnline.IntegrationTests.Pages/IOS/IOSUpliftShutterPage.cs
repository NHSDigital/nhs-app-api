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
            PageContent = new UpliftShutterPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSUpliftShutterPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSUpliftShutterPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSUpliftShutterPage Continue()
        {
            PageContent.ProveYourIdentityContinue();
            return this;
        }
    }
}
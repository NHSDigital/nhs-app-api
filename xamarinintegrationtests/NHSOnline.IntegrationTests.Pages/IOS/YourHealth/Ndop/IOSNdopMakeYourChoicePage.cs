using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth.Ndop
{
    public class IOSNdopMakeYourChoicePage
    {
        private IOSNdopMakeYourChoicePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NdopMakeYourChoicePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public NdopMakeYourChoicePageContent PageContent { get; }

        public static IOSNdopMakeYourChoicePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNdopMakeYourChoicePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
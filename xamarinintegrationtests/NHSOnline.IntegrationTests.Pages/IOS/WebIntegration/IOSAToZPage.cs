using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSAToZPage
    {
        private IOSAToZPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AToZPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public AToZPageContent PageContent { get; }

        public static IOSAToZPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAToZPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSAToZPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}
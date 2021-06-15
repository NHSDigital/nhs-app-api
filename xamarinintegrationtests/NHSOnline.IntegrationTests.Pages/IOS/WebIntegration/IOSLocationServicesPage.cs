using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSLocationServicesPage
    {

        private IOSLocationServicesPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new LocationServicesPageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        private IOSFullNavigation Navigation { get; }

        public LocationServicesPageContent PageContent { get; }

        public static IOSLocationServicesPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLocationServicesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSLocationServicesPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}
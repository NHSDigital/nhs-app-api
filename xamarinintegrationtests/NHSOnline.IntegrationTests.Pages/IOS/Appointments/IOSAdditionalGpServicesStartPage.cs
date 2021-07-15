using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSAdditionalGpServicesStartPage
    {
        private IOSAdditionalGpServicesStartPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AdditionalGpServicesStartPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public AdditionalGpServicesStartPageContent PageContent { get; }

        public static IOSAdditionalGpServicesStartPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAdditionalGpServicesStartPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
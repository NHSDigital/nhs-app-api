using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSAdditionalGpServicesConditionsPage
    {
        private IOSAdditionalGpServicesConditionsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AdditionalGpServicesConditionsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public AdditionalGpServicesConditionsPageContent PageContent { get; }

        public static IOSAdditionalGpServicesConditionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAdditionalGpServicesConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
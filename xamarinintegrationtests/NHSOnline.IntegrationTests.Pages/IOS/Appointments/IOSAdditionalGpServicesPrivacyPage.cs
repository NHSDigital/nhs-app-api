using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSAdditionalGpServicesPrivacyPage
    {
        private IOSAdditionalGpServicesPrivacyPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AdditionalGpServicesPrivacyPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        public AdditionalGpServicesPrivacyPageContent PageContent { get; }

        public static IOSAdditionalGpServicesPrivacyPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAdditionalGpServicesPrivacyPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
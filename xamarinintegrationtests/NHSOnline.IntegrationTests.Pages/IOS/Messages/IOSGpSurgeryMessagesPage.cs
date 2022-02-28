using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Messages
{
    public class IOSGpSurgeryMessagesPage
    {
        private IOSGpSurgeryMessagesPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new GpSurgeryMessagesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        private GpSurgeryMessagesPageContent PageContent { get; }

        public static IOSGpSurgeryMessagesPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSGpSurgeryMessagesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSManageNotificationsPromptPage
    {
        public ManageNotificationsPromptPageContent PageContent { get; }

        private IOSManageNotificationsPromptPage(IIOSDriverWrapper driver)
        {
            PageContent = new ManageNotificationsPromptPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static IOSManageNotificationsPromptPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSManageNotificationsPromptPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
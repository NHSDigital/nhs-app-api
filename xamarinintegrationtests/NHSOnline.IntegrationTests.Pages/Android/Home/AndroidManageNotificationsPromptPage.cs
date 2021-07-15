using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidManageNotificationsPromptPage
    {
        public ManageNotificationsPromptPageContent PageContent { get; }

        private AndroidManageNotificationsPromptPage(IAndroidDriverWrapper driver)
        {
            PageContent = new ManageNotificationsPromptPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static AndroidManageNotificationsPromptPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidManageNotificationsPromptPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
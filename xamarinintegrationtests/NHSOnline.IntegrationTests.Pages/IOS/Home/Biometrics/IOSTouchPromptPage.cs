using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.Biometrics
{
    public class IOSTouchPromptPage
    {
        private IOSTouchPromptPage(IIOSDriverWrapper driver)
        {
            var webInteractor = driver.Web.NhsAppPreHomeWebView();

            PageContent = new TouchRegistrationPageContent(webInteractor);
        }

        public TouchRegistrationPageContent PageContent { get; }


        public static IOSTouchPromptPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSTouchPromptPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements() => PageContent.AssertPageElements();
    }
}
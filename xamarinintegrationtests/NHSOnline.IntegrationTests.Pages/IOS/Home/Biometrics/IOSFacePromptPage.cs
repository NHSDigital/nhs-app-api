using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.Biometrics
{
    public class IOSFacePromptPage
    {
        private IOSFacePromptPage(IIOSDriverWrapper driver)
        {
            var webInteractor = driver.Web.NhsAppPreHomeWebView();

            PageContent = new FaceIdRegistrationPromptPageContent(webInteractor);
        }

        public FaceIdRegistrationPromptPageContent PageContent { get; }


        public static IOSFacePromptPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSFacePromptPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements() => PageContent.AssertPageElements();
    }
}
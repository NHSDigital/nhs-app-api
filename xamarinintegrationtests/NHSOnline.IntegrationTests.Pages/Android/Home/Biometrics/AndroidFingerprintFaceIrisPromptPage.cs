using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home.Biometrics
{
    public class AndroidFingerprintFaceIrisPromptPage
    {
        public FingerprintFaceIrisPromptPageContent PageContent { get; }

        private AndroidFingerprintFaceIrisPromptPage(IAndroidDriverWrapper driver)
        {
            PageContent = new FingerprintFaceIrisPromptPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static AndroidFingerprintFaceIrisPromptPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidFingerprintFaceIrisPromptPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
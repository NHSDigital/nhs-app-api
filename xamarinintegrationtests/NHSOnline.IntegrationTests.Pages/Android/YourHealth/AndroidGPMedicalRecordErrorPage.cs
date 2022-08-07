using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public sealed class AndroidGPMedicalRecordErrorPage
    {
        private AndroidGPMedicalRecordErrorPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new GpMedicalRecordErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        private GpMedicalRecordErrorPageContent PageContent { get; }

        public static AndroidGPMedicalRecordErrorPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false, bool postTryAgain = false)
        {
            var page = new AndroidGPMedicalRecordErrorPage(driver);
            page.PageContent.AssertOnPage(postTryAgain);

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidGpMedicalRecordPage));
            }

            return page;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
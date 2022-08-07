using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public sealed class IOSGpMedicalRecordErrorPage
    {
        private IOSGpMedicalRecordErrorPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new GpMedicalRecordErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public GpMedicalRecordErrorPageContent PageContent { get; }

        public static IOSGpMedicalRecordErrorPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false, bool postTryAgain = false)
        {
            var page = new IOSGpMedicalRecordErrorPage(driver);
            page.PageContent.AssertOnPage(postTryAgain);
            if (screenshot)
            {
                driver.Screenshot(nameof(IOSGpMedicalRecordErrorPage));
            }
            return page;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public sealed class IOSGpMedicalRecordPage
    {
        private IOSGpMedicalRecordPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new GpMedicalRecordPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public GpMedicalRecordPageContent PageContent { get; }

        public static IOSGpMedicalRecordPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSGpMedicalRecordPage(driver);
            page.PageContent.AssertOnPage();
            if (screenshot)
            {
                driver.Screenshot(nameof(IOSGpMedicalRecordPage));
            }
            return page;
        }
    }
}
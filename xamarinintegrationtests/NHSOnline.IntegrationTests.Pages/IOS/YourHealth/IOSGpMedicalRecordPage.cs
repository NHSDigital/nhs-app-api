using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
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

        public static IOSGpMedicalRecordPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSGpMedicalRecordPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
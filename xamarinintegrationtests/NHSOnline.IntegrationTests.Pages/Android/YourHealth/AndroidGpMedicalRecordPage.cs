using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public sealed class AndroidGpMedicalRecordPage
    {
        private AndroidGpMedicalRecordPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new GpMedicalRecordPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        public GpMedicalRecordPageContent PageContent { get; }

        public static AndroidGpMedicalRecordPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGpMedicalRecordPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}

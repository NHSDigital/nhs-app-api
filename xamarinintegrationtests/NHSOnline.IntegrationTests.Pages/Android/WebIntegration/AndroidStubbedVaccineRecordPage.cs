using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidStubbedVaccineRecordPage
    {
        private AndroidStubbedVaccineRecordPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new VaccineRecordPageContent(driver.Web.WebIntegrationWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public VaccineRecordPageContent PageContent { get; }

        public static AndroidStubbedVaccineRecordPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedVaccineRecordPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
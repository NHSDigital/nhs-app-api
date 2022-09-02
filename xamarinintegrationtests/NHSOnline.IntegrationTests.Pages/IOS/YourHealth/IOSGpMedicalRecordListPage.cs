using NHSOnline.HttpMocks.GpMedicalRecord;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public class IOSGpMedicalRecordListPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSGpMedicalRecordListPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new GpMedicalRecordListPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        private GpMedicalRecordListPageContent PageContent { get; }

        private IOSLink DocumentsLink => IOSLink.WhichContains(_driver, "Documents");

        public static IOSGpMedicalRecordListPage AssertOnPage(
            IIOSDriverWrapper driver,
            bool screenshot = false,
            CareRecordLevel careRecordLevel = CareRecordLevel.Summary)
        {
            var page = new IOSGpMedicalRecordListPage(driver);
            page.PageContent.AssertOnPage(careRecordLevel);

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSGpMedicalRecordListPage));
            }
            return page;
        }

        public void DocumentsClick() => DocumentsLink.Touch();
    }
}
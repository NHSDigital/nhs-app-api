using System.Globalization;
using NHSOnline.HttpMocks.GpMedicalRecord;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public class IOSGpMedicalRecordDocumentsPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSGpMedicalRecordDocumentsPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
        }

        private IOSFullNavigation Navigation { get; }

        public IOSLink DocumentDetailLink => IOSLink.WhichContains(_driver,
            $"Document added on {GpMedicalRecordConstants.TenMonthsAgoDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}");

        private IOSFilesAppHeader DocumentNotAvailable => IOSFilesAppHeader.WithText(_driver,
            $"The document added on {GpMedicalRecordConstants.TenMonthsAgoDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)} is not available - NHS App");

        public static IOSGpMedicalRecordDocumentsPage AssertOnPage(
            IIOSDriverWrapper driver,
            bool screenshot = false,
            bool isDocumentNotAvailableView = false)
        {
            var page = new IOSGpMedicalRecordDocumentsPage(driver);

            if (isDocumentNotAvailableView)
            {
                page.DocumentNotAvailable.AssertVisible();
            }
            else
            {
                page.DocumentDetailLink.AssertVisible();
            }

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSGpMedicalRecordDocumentsPage));
            }
            return page;
        }
    }
}
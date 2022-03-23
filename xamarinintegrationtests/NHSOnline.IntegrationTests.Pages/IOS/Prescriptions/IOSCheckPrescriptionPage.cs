using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSCheckPrescriptionPage
    {
        private readonly IIOSDriverWrapper _driver;

        public CheckPrescriptionPageContent PageContent { get; }

        private IOSCheckPrescriptionPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            PageContent = new CheckPrescriptionPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSCheckPrescriptionPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSCheckPrescriptionPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSCheckPrescriptionPage));
            }

            return page;
        }
    }
}
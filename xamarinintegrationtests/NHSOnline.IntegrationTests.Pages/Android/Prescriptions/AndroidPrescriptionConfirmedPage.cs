using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public class AndroidPrescriptionConfirmedPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }

        public PrescriptionConfirmedPageContent PageContent { get; }

        private AndroidPrescriptionConfirmedPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PrescriptionConfirmedPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidPrescriptionConfirmedPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidPrescriptionConfirmedPage));
            }
        }
    }
}
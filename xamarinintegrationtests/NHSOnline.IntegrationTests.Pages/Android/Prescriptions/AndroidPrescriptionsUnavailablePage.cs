using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public sealed class AndroidPrescriptionsUnavailablePage
    {
        private AndroidFullNavigation Navigation { get; }
        public PrescriptionsUnavailablePageContent PageContent { get; }

        private AndroidPrescriptionsUnavailablePage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PrescriptionsUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidPrescriptionsUnavailablePage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidPrescriptionsUnavailablePage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidPrescriptionsUnavailablePage));
            }

            return page;
        }

        public AndroidPrescriptionsUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}
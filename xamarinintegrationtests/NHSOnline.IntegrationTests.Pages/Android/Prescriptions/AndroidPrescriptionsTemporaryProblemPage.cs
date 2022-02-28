using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public sealed class AndroidPrescriptionsTemporaryProblemPage
    {
        private AndroidFullNavigation Navigation { get; }
        private PrescriptionsTemporaryProblemPageContent PageContent { get; }

        private AndroidPrescriptionsTemporaryProblemPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PrescriptionsTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidPrescriptionsTemporaryProblemPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPrescriptionsTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidPrescriptionsTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}
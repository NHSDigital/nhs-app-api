using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public sealed class AndroidPrescriptionsPage
    {
        private AndroidPrescriptionsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PrescriptionsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        private PrescriptionsPageContent PageContent { get; }

        public static AndroidPrescriptionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPrescriptionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}

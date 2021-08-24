using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAdditionalGpServicesPage
    {
        public AdditionalGpServicesPageContent PageContent { get; }

        private AndroidAdditionalGpServicesPage(IAndroidDriverWrapper driver)
        {
            PageContent = new AdditionalGpServicesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAdditionalGpServicesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdditionalGpServicesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
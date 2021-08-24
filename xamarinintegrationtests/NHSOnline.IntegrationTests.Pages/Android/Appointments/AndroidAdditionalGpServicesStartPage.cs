using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAdditionalGpServicesStartPage
    {
        public AndroidFullNavigation Navigation { get; }
        public AdditionalGpServicesStartPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAdditionalGpServicesStartPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AdditionalGpServicesStartPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAdditionalGpServicesStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdditionalGpServicesStartPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
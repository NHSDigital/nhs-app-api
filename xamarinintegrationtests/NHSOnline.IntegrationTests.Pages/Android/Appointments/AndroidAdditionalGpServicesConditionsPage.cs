using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAdditionalGpServicesConditionsPage
    {
        public AndroidFullNavigation Navigation { get; }
        public AdditionalGpServicesConditionsPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAdditionalGpServicesConditionsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AdditionalGpServicesConditionsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidAdditionalGpServicesConditionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdditionalGpServicesConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAdditionalGpServicesPage
    {
        public AdditionalGpServicesPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAdditionalGpServicesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            PageContent = new AdditionalGpServicesPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidAdditionalGpServicesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdditionalGpServicesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
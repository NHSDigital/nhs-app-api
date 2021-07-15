using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidAdditionalGpServicesPrivacyPage
    {
        public AndroidFullNavigation Navigation { get; }
        public AdditionalGpServicesPrivacyPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAdditionalGpServicesPrivacyPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AdditionalGpServicesPrivacyPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAdditionalGpServicesPrivacyPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdditionalGpServicesPrivacyPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
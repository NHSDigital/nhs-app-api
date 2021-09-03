using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings
{
    public class AndroidFingerprintFaceIrisPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFingerprintFaceIrisPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new FingerprintFaceIrisRegistrationPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public FingerprintFaceIrisRegistrationPageContent PageContent { get; }

        public static AndroidFingerprintFaceIrisPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidFingerprintFaceIrisPage(driver);
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
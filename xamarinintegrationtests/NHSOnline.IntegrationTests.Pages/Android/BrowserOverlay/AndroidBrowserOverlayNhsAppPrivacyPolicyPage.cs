using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayNhsAppPrivacyPolicyPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "NHS App privacy policy";

        private AndroidBrowserOverlayNhsAppPrivacyPolicyPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayNhsAppPrivacyPolicyPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayNhsAppPrivacyPolicyPage(browserOverlay);
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
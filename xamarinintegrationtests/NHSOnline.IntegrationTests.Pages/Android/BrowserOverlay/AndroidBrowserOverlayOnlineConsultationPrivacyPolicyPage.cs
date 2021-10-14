using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayOnlineConsultationPrivacyPolicyPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "Online Consultations";

        private AndroidBrowserOverlayOnlineConsultationPrivacyPolicyPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayOnlineConsultationPrivacyPolicyPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayOnlineConsultationPrivacyPolicyPage(browserOverlay);
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
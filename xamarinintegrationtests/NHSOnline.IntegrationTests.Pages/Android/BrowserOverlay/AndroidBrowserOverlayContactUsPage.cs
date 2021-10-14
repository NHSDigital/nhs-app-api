using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayContactUsPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "Contact Us";

        private AndroidBrowserOverlayContactUsPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayContactUsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayContactUsPage(browserOverlay);
        }

        public AndroidBrowserOverlayContactUsPage AssertErrorCode(string errorCode)
        {
            _androidBrowserOverlay.AssertErrorCode(errorCode);
            return this;
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
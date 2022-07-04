using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayDigitalCovidPassPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "Digital Covid Pass";

        private AndroidBrowserOverlayDigitalCovidPassPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayDigitalCovidPassPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayDigitalCovidPassPage(browserOverlay);
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
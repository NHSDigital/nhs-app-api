using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayCovidPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "Covid";

        private AndroidBrowserOverlayCovidPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayCovidPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayCovidPage(browserOverlay);
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlayNhsAppHelpPage
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private AndroidBrowserOverlayNhsAppHelpPage(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlayNhsAppHelpPage AssertOnPage(IAndroidDriverWrapper driver, string textToMatch)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, $"Help Page for {textToMatch}");
            browserOverlay.AssertOnPage();
            return new AndroidBrowserOverlayNhsAppHelpPage(browserOverlay);
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();
    }
}
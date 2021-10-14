using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlay111Page
    {
        private readonly AndroidBrowserOverlay _androidBrowserOverlay;

        private const string Title = "111";

        private AndroidBrowserOverlay111Page(AndroidBrowserOverlay androidBrowserOverlay)
        {
            _androidBrowserOverlay = androidBrowserOverlay;
        }

        public static AndroidBrowserOverlay111Page AssertInBrowserOverlay(IAndroidDriverWrapper driver)
        {
            var browserOverlay = AndroidBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            return new AndroidBrowserOverlay111Page(browserOverlay);
        }

        public AndroidBrowserOverlay111Page AssertOnPage()
        {
            _androidBrowserOverlay.AssertOnPage();
            return this;
        }

        public void ReturnToApp() => _androidBrowserOverlay.ReturnToApp();

        public void AssertNoInternet() => _androidBrowserOverlay.AssertNoInternet();
    }
}
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlay111Page
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private const string Title = "111";

        private IOSBrowserOverlay111Page(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlay111Page AssertInBrowserOverlay(IIOSDriverWrapper driver)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            return new IOSBrowserOverlay111Page(browserOverlay);
        }

        public IOSBrowserOverlay111Page AssertOnPage()
        {
            _iOsBrowserOverlay.AssertOnPage();
            return this;
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();

        public void AssertNoInternet() => _iOsBrowserOverlay.AssertNoInternet();
    }
}
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlayCovidPage
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private const string Title = "Covid";

        private IOSBrowserOverlayCovidPage(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlayCovidPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new IOSBrowserOverlayCovidPage(browserOverlay);
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();
    }
}
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlayNhsAppHelpPage
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private IOSBrowserOverlayNhsAppHelpPage(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlayNhsAppHelpPage AssertOnPage(IIOSDriverWrapper driver, string textToMatch)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, $"Help Page for {textToMatch}");
            browserOverlay.AssertOnPage();
            return new IOSBrowserOverlayNhsAppHelpPage(browserOverlay);
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();
    }
}
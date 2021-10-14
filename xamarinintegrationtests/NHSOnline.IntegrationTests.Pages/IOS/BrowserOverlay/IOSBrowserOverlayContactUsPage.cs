using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlayContactUsPage
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private const string Title = "Contact Us";

        private IOSBrowserOverlayContactUsPage(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlayContactUsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new IOSBrowserOverlayContactUsPage(browserOverlay);
        }

        public IOSBrowserOverlayContactUsPage AssertErrorCode(string errorCode)
        {
            _iOsBrowserOverlay.AssertErrorCode(errorCode);
            return this;
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();
    }
}
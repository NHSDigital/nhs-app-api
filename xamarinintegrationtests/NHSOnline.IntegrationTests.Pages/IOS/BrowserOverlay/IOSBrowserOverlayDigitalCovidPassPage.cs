using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlayDigitalCovidPassPage
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private const string Title = "Digital Covid Pass";

        private IOSBrowserOverlayDigitalCovidPassPage(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlayDigitalCovidPassPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new IOSBrowserOverlayDigitalCovidPassPage(browserOverlay);
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();
    }
}
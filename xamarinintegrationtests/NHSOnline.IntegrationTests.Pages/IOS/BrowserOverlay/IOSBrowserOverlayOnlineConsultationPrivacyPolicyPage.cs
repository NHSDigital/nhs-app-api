using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlayOnlineConsultationPrivacyPolicyPage
    {
        private readonly IOSBrowserOverlay _iOsBrowserOverlay;

        private const string Title = "Online Consultations";

        private IOSBrowserOverlayOnlineConsultationPrivacyPolicyPage(IOSBrowserOverlay iOsBrowserOverlay)
        {
            _iOsBrowserOverlay = iOsBrowserOverlay;
        }

        public static IOSBrowserOverlayOnlineConsultationPrivacyPolicyPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var browserOverlay = IOSBrowserOverlay.AssertInBrowserOverlay(driver, Title);
            browserOverlay.AssertOnPage();
            return new IOSBrowserOverlayOnlineConsultationPrivacyPolicyPage(browserOverlay);
        }

        public void ReturnToApp() => _iOsBrowserOverlay.ReturnToApp();
    }
}
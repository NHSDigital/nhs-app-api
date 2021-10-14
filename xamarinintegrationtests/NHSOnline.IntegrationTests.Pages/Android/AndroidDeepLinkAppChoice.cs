using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidDeepLinkAppChoice
    {
        private AndroidBrowserChoice AndroidBrowserChoice { get; }

        private AndroidDeepLinkAppChoice(IAndroidDriverWrapper driver)
        {
            AndroidBrowserChoice = new AndroidBrowserChoice(driver, "NHS App - BrowserStack", "web.local.bitraft.io");
        }
        
        public static AndroidDeepLinkAppChoice AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var appChoice = new AndroidDeepLinkAppChoice(driver);
            appChoice.AndroidBrowserChoice.AssertDisplayed();
            return appChoice;
        }

        public void ChooseNhsApp()
        {
            AndroidBrowserChoice.ChooseTargetApp();
        }
    }
}
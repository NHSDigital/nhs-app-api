using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidDeepLinkAppChoice
    {
        private AndroidAppChoice AndroidAppChoice { get; }

        private AndroidDeepLinkAppChoice(IAndroidDriverWrapper driver)
        {
            AndroidAppChoice = new AndroidAppChoice(driver, "NHS App - BrowserStack", "web.local.bitraft.io");
        }
        
        public static AndroidDeepLinkAppChoice AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var appChoice = new AndroidDeepLinkAppChoice(driver);
            appChoice.AndroidAppChoice.AssertDisplayed();
            return appChoice;
        }

        public void ChooseNhsApp()
        {
            AndroidAppChoice.ChooseTargetApp();
        }
    }
}
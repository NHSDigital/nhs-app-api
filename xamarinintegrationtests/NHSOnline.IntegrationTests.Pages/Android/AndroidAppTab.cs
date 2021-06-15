using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidAppTab
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _text;

        private AndroidAppTab(IAndroidDriverWrapper driver, string text = "")
        {
            _driver = driver;
            _text = text;
        }

        private AndroidAppBrowserTabContents Contents => AndroidAppBrowserTabContents.WithText(_driver, _text);

        private AndroidImageButton AndroidAppTabClose => AndroidImageButton.WithDescription(_driver, "Close tab");

        public static AndroidAppTab AssertOnCovidPage(IAndroidDriverWrapper driver) => AssertOnPageByTitle(driver, "Covid");

        public static AndroidAppTab AssertOnCovidAppPage(IAndroidDriverWrapper driver) => AssertOnPageByTitle(driver, "CovidApp");

        public static AndroidAppTab AssertOnLoginHelpPage(IAndroidDriverWrapper driver) => AssertOnPageByTitle(driver, "Login Help");

        public static AndroidAppTab AssertOnHomeHelpPage(IAndroidDriverWrapper driver) => AssertOnPageByTitle(driver, "Home Help");

        public static AndroidAppTab AssertOnContactUsPage(IAndroidDriverWrapper driver) => AssertOnPageByTitle(driver, "Contact Us");

        public static AndroidAppTab AssertInBrowserAppTab(IAndroidDriverWrapper driver) => AssertOnPageByClose(driver);

        public void ReturnToApp() => AndroidAppTabClose.Click();

        private static AndroidAppTab AssertOnPageByTitle(IAndroidDriverWrapper driver, string title)
        {
            var androidAppTab = new AndroidAppTab(driver, title);
            androidAppTab.Contents.AssertVisible();
            return androidAppTab;
        }

        private static AndroidAppTab AssertOnPageByClose(IAndroidDriverWrapper driver)
        {
            var androidAppTab = new AndroidAppTab(driver);
            androidAppTab.AndroidAppTabClose.AssertVisible();
            return androidAppTab;
        }
    }
}

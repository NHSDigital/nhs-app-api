using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidAppTab
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _text;

        private AndroidAppTab(IAndroidDriverWrapper driver, string text)
        {
            _driver = driver;
            _text = text;
        }

        private AndroidAppBrowserTabContents Contents => AndroidAppBrowserTabContents.WithText(_driver, _text);

        private AndroidImageButton AndroidAppTabClose => AndroidImageButton.WithDescription(_driver, "Close tab");

        public static AndroidAppTab AssertOnConditionsPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Conditions");

        public static AndroidAppTab AssertOnCovidPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Covid");

        public static AndroidAppTab AssertOnCovidAppPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "CovidApp");

        public static AndroidAppTab AssertOnCovidConditionsPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Covid Conditions");

        public static AndroidAppTab AssertOn111Page(IAndroidDriverWrapper driver) => AssertOnPage(driver, "111");

        public static AndroidAppTab AssertOnLoginHelpPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Login Help");

        public static AndroidAppTab AssertOnHomeHelpPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Home Help");

        public static AndroidAppTab AssertOnContactUsPage(IAndroidDriverWrapper driver) => AssertOnPage(driver, "Contact Us");

        public void ReturnToApp() => AndroidAppTabClose.Click();

        private static AndroidAppTab AssertOnPage(IAndroidDriverWrapper driver, string title)
        {
            var androidAppTab = new AndroidAppTab(driver, title);
            androidAppTab.Contents.AssertVisible();
            return androidAppTab;
        }
    }
}

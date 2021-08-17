using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public static AndroidAppTab AssertOnCovidPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Covid");

        public static AndroidAppTab AssertOnHelpPageByTitle(IAndroidDriverWrapper driver, string titleToMatch)
            => AssertOnPageByTitle(driver, $"{titleToMatch} Help");

        public static AndroidAppTab AssertOnHelpPageByText(IAndroidDriverWrapper driver, string textToMatch)
            => AssertOnPageByTitle(driver, $"Help Page for {textToMatch}");

        public static AndroidAppTab AssertOnContactUsPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Contact Us");

        public static AndroidAppTab AssertOnOnlineConsultationPrivacyPolicyPage(IAndroidDriverWrapper driver) =>
            AssertOnPageByTitle(driver, "NHS App privacy policy: online consultation services");

        public static AndroidAppTab AssertOnPrivacyPolicyPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "NHS App privacy policy");

        public static AndroidAppTab AssertInBrowserAppTab(IAndroidDriverWrapper driver) => AssertOnPageByClose(driver);

        private AndroidButton AcceptNhsCookiesButton => AndroidButton.WithText(_driver, "I'm OK with analytics cookies");

        public void ReturnToApp() => AndroidAppTabClose.Click();

        private static AndroidAppTab AssertOnPageByTitle(IAndroidDriverWrapper driver, string title)
        {
            var androidAppTab = new AndroidAppTab(driver, title);

            try
            {
                androidAppTab.Contents.AssertVisible();
            }
            catch (AssertFailedException)
            {
                androidAppTab.AcceptNhsCookiesButton.Click();
                androidAppTab.Contents.AssertVisible();
            }

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

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

        private AndroidAppBrowserTabContents Title => AndroidAppBrowserTabContents.WithText(_driver, _text);

        private AndroidAppBrowserTabContents ErrorCodeText(string errorCodeSubstring) =>
            AndroidAppBrowserTabContents.WithText(_driver, errorCodeSubstring);

        private AndroidAppBrowserTabContents NoInternetText => AndroidAppBrowserTabContents.WithText(_driver, "No internet");

        private AndroidImageButton AndroidAppTabClose => AndroidImageButton.WithDescription(_driver, "Close tab");

        private AndroidButton AcceptNhsCookiesButton => AndroidButton.WithText(_driver, "I'm OK with analytics cookies");

        public void AssertNoInternet() => NoInternetText.AssertVisible();

        public void ReturnToApp() => AndroidAppTabClose.Click();

        public static AndroidAppTab AssertOnCovidPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Covid");

        public static AndroidAppTab AssertOn111Page(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "111");

        public static AndroidAppTab AssertOnHelpPageByText(IAndroidDriverWrapper driver, string textToMatch)
            => AssertOnPageByTitle(driver, $"Help Page for {textToMatch}");

        public static AndroidAppTab AssertOnContactUsPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Contact Us");

        public static AndroidAppTab AssertOnContactUsPageByErrorCode(IAndroidDriverWrapper driver, string errorCode)
            => AssertOnPageByTextContaining(driver, "Contact Us", $"errorcode: {errorCode}");

        public static AndroidAppTab AssertOnOnlineConsultationPrivacyPolicyPage(IAndroidDriverWrapper driver) =>
            AssertOnPageByTitle(driver, "Online Consultations");

        public static AndroidAppTab AssertOnPrivacyPolicyPage(IAndroidDriverWrapper driver)
            => AssertOnPageByTitle(driver, "NHS App privacy policy");

        public static AndroidAppTab AssertInBrowserAppTab(IAndroidDriverWrapper driver)
            => AssertOnPageByClose(driver);

        private static AndroidAppTab AssertOnPageByTitle(IAndroidDriverWrapper driver, string title)
        {
            var androidAppTab = new AndroidAppTab(driver, title);

            try
            {
                androidAppTab.Title.AssertVisible();
            }
            catch (AssertFailedException)
            {
                androidAppTab.AcceptNhsCookiesButton.Click();
                androidAppTab.Title.AssertVisible();
            }

            return androidAppTab;
        }

        private static AndroidAppTab AssertOnPageByTextContaining(IAndroidDriverWrapper driver, string title, string subString)
        {
            var androidAppTab = new AndroidAppTab(driver, title);
            androidAppTab.ErrorCodeText(subString).AssertSubStringVisible();
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

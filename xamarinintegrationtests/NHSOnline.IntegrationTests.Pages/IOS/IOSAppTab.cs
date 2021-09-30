using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSAppTab
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSAppTab(IIOSDriverWrapper driver, string title = "")
        {
            _driver = driver;
            _title = title;
        }

        private IOSButton DoneButton => IOSButton.WithText(_driver, "Done");

        private IOSLabel TitleText => IOSLabel.WithText(_driver, _title);

        private WebText ErrorCodeText(IWebInteractor webInteractor, string errorCode) => WebText.WithText(webInteractor, errorCode);

        private IOSLabel NoInternetText => IOSLabel.WithText(_driver, "Safari cannot open the page because your iPhone is not connected to the internet.");

        private IOSButton AcceptAnalyticsCookiesButton => IOSButton.WithText(_driver, "I'm OK with analytics cookies");

        public void AssertNoInternet() => NoInternetText.AssertVisible();

        public void ReturnToApp() => DoneButton.Click();

        public static IOSAppTab AssertOnCovidPage(IIOSDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Covid");

        public static IOSAppTab AssertOn111Page(IIOSDriverWrapper driver)
            => AssertOnPageByTitle(driver, "111");

        public static IOSAppTab AssertOnContactUsPage(IIOSDriverWrapper driver)
            => AssertOnPageByTitle(driver, "Contact Us");

        public static IOSAppTab AssertOnContactUsPageByErrorCode(IIOSDriverWrapper driver, string errorCode)
            => AssertOnPageByTextContaining(driver, $"errorcode: {errorCode}");

        public static IOSAppTab AssertOnHelpPageByText(IIOSDriverWrapper driver, string textToMatch)
            => AssertOnPageByTitle(driver, $"Help Page for {textToMatch}");

        public static IOSAppTab AssertInBrowserAppTab(IIOSDriverWrapper driver)
            => AssertOnPageByClose(driver);

        public static IOSAppTab AssertOnOnlineConsultationPrivacyPolicyPage(IIOSDriverWrapper driver)
            => AssertOnPageByTitle(driver, "NHS App privacy policy: online consultation services");

        private static IOSAppTab AssertOnPageByTitle(IIOSDriverWrapper driver, string title)
        {
            var page = new IOSAppTab(driver, title);

            try
            {
                page.TitleText.AssertVisible();
            }
            catch (AssertFailedException)
            {
                page.AcceptAnalyticsCookiesButton.Click();
                page.TitleText.AssertVisible();
            }

            return page;
        }

        private static IOSAppTab AssertOnPageByTextContaining(IIOSDriverWrapper driver, string subString)
        {
            var iosAppTab = new IOSAppTab(driver);
            iosAppTab.ErrorCodeText(driver.Web.BrowserOverlayWebView(), subString).AssertContainsVisible();
            return iosAppTab;
        }

        private static IOSAppTab AssertOnPageByClose(IIOSDriverWrapper driver)
        {
            var page = new IOSAppTab(driver);
            page.DoneButton.AssertVisible();
            return page;
        }
    }
}

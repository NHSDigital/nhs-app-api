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

        private WebText TitleText(IWebInteractor webInteractor) => WebText.WithTagAndText(webInteractor, "h1", _title);

        public void ReturnToApp() => DoneButton.Click();

        public static IOSAppTab AssertOnConditionsPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Conditions");

        public static IOSAppTab AssertOnCovidPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Covid");

        public static IOSAppTab AssertOn111Page(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "111");

        public static IOSAppTab AssertOnCovidConditionsPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Covid Conditions");

        public static IOSAppTab AssertOnLoginHelpPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Login Help");

        public static IOSAppTab AssertOnContactUsPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Contact Us");

        public static IOSAppTab AssertOnHomeHelpPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "Home Help");

        public static IOSAppTab AssertInBrowserAppTab(IIOSDriverWrapper driver) => AssertOnPageByClose(driver);

        public static IOSAppTab AssertOnOnlineConsultationPrivacyPolicyPage(IIOSDriverWrapper driver) =>
            AssertOnPageByTitle(driver, "NHS App privacy policy: online consultation services");

        public static IOSAppTab AssertOnPrivacyPolicyPage(IIOSDriverWrapper driver) => AssertOnPageByTitle(driver, "NHS App privacy policy");

        private WebButton AcceptAnalyticsCookiesButton(IWebInteractor webInteractor) => WebButton.WithText(webInteractor, "I'm OK with analytics cookies");

        private static IOSAppTab AssertOnPageByTitle(IIOSDriverWrapper driver, string title)
        {
            var page = new IOSAppTab(driver, title);
            try
            {
                page.TitleText(driver.Web.BrowserOverlayWebView()).AssertVisible();
            }
            catch (AssertFailedException)
            {
                page.AcceptAnalyticsCookiesButton(driver.Web.BrowserOverlayWebView()).Click();
                page.TitleText(driver.Web.BrowserOverlayWebView()).AssertVisible();
            }

            return page;
        }

        private static IOSAppTab AssertOnPageByClose(IIOSDriverWrapper driver)
        {
            var page = new IOSAppTab(driver);
            page.DoneButton.AssertVisible();

            return page;
        }
    }
}

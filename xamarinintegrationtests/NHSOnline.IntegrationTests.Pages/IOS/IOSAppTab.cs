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

        private IOSButton DoneButton => new IOSButton(_driver, "Done");

        private WebText TitleText(IWebInteractor webInteractor) => new WebText(webInteractor, "h1", _title);

        public void ReturnToApp() => DoneButton.Click();

        public static IOSAppTab AssertOnConditionsPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Conditions");

        public static IOSAppTab AssertOnCovidPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Covid");

        public static IOSAppTab AssertOn111Page(IIOSDriverWrapper driver) => AssertOnPage(driver, "111");

        public static IOSAppTab AssertOnCovidConditionsPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Covid Conditions");

        public static IOSAppTab AssertOnLoginHelpPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Login Help");

        public static IOSAppTab AssertOnContactUsPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Contact Us");

        private static IOSAppTab AssertOnPage(IIOSDriverWrapper driver, string title)
        {
            var page = new IOSAppTab(driver, title);
            page.TitleText(driver.Web(WebViewContext.OneOff)).AssertVisible();

            return page;
        }
    }
}
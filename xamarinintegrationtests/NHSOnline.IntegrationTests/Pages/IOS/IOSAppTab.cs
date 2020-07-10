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

        internal void ReturnToApp() => DoneButton.Click();

        internal static IOSAppTab AssertOnConditionsPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Conditions");

        internal static IOSAppTab AssertOnCovidPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Covid");

        internal static IOSAppTab AssertOn111Page(IIOSDriverWrapper driver) => AssertOnPage(driver, "111");

        internal static IOSAppTab AssertOnCovidConditionsPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Covid Conditions");

        internal static IOSAppTab AssertOnLoginHelpPage(IIOSDriverWrapper driver) => AssertOnPage(driver, "Login Help");

        private static IOSAppTab AssertOnPage(IIOSDriverWrapper driver, string title)
        {
            var page = new IOSAppTab(driver, title);

            using (var webInteractor = driver.Web(WebViewContext.OneOff))
            {
                page.TitleText(webInteractor).AssertVisible();
            }

            return page;
        }
    }
}
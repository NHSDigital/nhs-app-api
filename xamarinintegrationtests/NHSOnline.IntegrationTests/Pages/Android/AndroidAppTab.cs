using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidAppTab
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _title;

        private AndroidAppTab(IAndroidDriverWrapper driver, string title = "")
        {
            _driver = driver;
            _title = title;
        }

        private AndroidView ConditionsPageView => new AndroidView(_driver, "Conditions");

        private AndroidView CovidPageView => new AndroidView(_driver, "Covid");

        private AndroidView CovidConditionsPageView => new AndroidView(_driver, "Covid Conditions");

        private AndroidView OneOneOnePageView => new AndroidView(_driver, "111");

        private AndroidView LoginHelpPageView => new AndroidView(_driver, "Login Help");
        
        private AndroidImageButton AndroidAppTabClose => new AndroidImageButton(_driver, "Close tab");

        internal static AndroidAppTab.BrowserChoice AssertOnBrowserChoice(IAndroidDriverWrapper driver)
        {
            var browserChoice = new BrowserChoice(driver);
            return browserChoice;
        }

        internal AndroidAppTab AssertOnConditionsPage() => AssertOnPage(ConditionsPageView);

        internal AndroidAppTab AssertOnCovidPage() => AssertOnPage(CovidPageView);

        internal AndroidAppTab AssertOnCovidConditionsPage() => AssertOnPage(CovidConditionsPageView);

        internal AndroidAppTab AssertOn111Page() => AssertOnPage(OneOneOnePageView);

        internal AndroidAppTab AssertOnLoginHelpPage() => AssertOnPage(LoginHelpPageView);

        internal void ReturnToApp() => AndroidAppTabClose.Click();

        private AndroidAppTab AssertOnPage(AndroidView pageView)
        {
            pageView.AssertVisible();
            return this;
        }

        internal sealed class BrowserChoice
        {
            private readonly IAndroidDriverWrapper _driver;

            public BrowserChoice(IAndroidDriverWrapper driver) => _driver = driver;

            private AndroidLabel ChromeOption => new AndroidLabel(_driver, "Chrome");
            private AndroidButton JustOnceButtom => new AndroidButton(_driver, "JUST ONCE");

            public BrowserChoice ChooseChrome()
            {
                ChromeOption.Click();
                return this;
            }

            internal AndroidAppTab JustOnce()
            {
                JustOnceButtom.Click();
                return new AndroidAppTab(_driver);
            }
        }
    }
}

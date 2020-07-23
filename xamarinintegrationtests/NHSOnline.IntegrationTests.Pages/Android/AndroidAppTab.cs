using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidAppTab
    {
        private readonly IAndroidDriverWrapper _driver;

        internal AndroidAppTab(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidView ConditionsPageView => new AndroidView(_driver, "Conditions");

        private AndroidView CovidPageView => new AndroidView(_driver, "Covid");

        private AndroidView CovidConditionsPageView => new AndroidView(_driver, "Covid Conditions");

        private AndroidView OneOneOnePageView => new AndroidView(_driver, "111");

        private AndroidView LoginHelpPageView => new AndroidView(_driver, "Login Help");

        private AndroidView HomeHelpPageView => new AndroidView(_driver, "Home Help");

        private AndroidView ContactUsPageView => new AndroidView(_driver, "Contact Us");

        private AndroidImageButton AndroidAppTabClose => new AndroidImageButton(_driver, "Close tab");

        public static AndroidAppTabBrowserChoice AssertOnBrowserChoice(IAndroidDriverWrapper driver)
            => new AndroidAppTabBrowserChoice(driver);

        public AndroidAppTab AssertOnConditionsPage() => AssertOnPage(ConditionsPageView);

        public AndroidAppTab AssertOnCovidPage() => AssertOnPage(CovidPageView);

        public AndroidAppTab AssertOnCovidConditionsPage() => AssertOnPage(CovidConditionsPageView);

        public AndroidAppTab AssertOn111Page() => AssertOnPage(OneOneOnePageView);

        public AndroidAppTab AssertOnLoginHelpPage() => AssertOnPage(LoginHelpPageView);

        public AndroidAppTab AssertOnHomeHelpPage() => AssertOnPage(HomeHelpPageView);

        public AndroidAppTab AssertOnContactUsPage() => AssertOnPage(ContactUsPageView);

        public void ReturnToApp() => AndroidAppTabClose.Click();

        private AndroidAppTab AssertOnPage(AndroidView pageView)
        {
            pageView.AssertVisible();
            return this;
        }
    }
}

using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel WelcomeMessage => new AndroidLabel(_driver, "Welcome to the NHS App");

        private AndroidButton ContinueButton => new AndroidButton(_driver, "CONTINUE WITH NHS LOGIN");

        internal static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
            page.WelcomeMessage.AssertVisible();
            return page;
        }

        public void ContinueWithNhsLogin()
        {
            ContinueButton.Click();
        }
    }
}
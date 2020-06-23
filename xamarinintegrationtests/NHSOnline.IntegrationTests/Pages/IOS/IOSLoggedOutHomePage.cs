using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel WelcomeMessage => new IOSLabel(_driver, "Welcome to the NHS App");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue with NHS Login");

        internal static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.WelcomeMessage.AssertVisible();
            return page;
        }

        public void ContinueWithNhsLogin()
        {
            ContinueButton.Click();
        }
    }
}
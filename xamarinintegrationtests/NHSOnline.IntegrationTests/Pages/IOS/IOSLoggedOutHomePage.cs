using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel HowAreYouFeelingText => new IOSLabel(_driver, "How are you feeling today?");
        private IOSLabel AccessServicesText => new IOSLabel(_driver, "To access your NHS services");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue with NHS login");
        private IOSButton CheckYourSymptoms => new IOSButton(_driver, "Check symptoms");

        internal static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.HowAreYouFeelingText.AssertVisible();
            page.AccessServicesText.AssertVisible();
            page.ContinueButton.AssertVisible();
            page.CheckYourSymptoms.AssertVisible();
            return page;
        }

        public void ContinueWithNhsLogin()
        {
            ContinueButton.Click();
        }
    }
}
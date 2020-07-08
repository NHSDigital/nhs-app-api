using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel CovidConditions => new IOSLabel(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");

        private IOSLabel HowAreYouFeelingText => new IOSLabel(_driver, "How are you feeling today?");
        private IOSLabel AccessServicesText => new IOSLabel(_driver, "To access your NHS services");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue with NHS login");
        private IOSButton CheckYourSymptoms => new IOSButton(_driver, "Check symptoms");

        private IOSIcon HelpIcon => new IOSIcon(_driver, "NHS app help icon");


        internal static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.HowAreYouFeelingText.AssertVisible();
            page.AccessServicesText.AssertVisible();
            page.ContinueButton.AssertVisible();
            page.CheckYourSymptoms.AssertVisible();
            return page;
        }

        internal IOSLoggedOutHomePage AssertCovidLinkOnPage()
        {
            CovidConditions.AssertVisible();
            return this;
        }

        internal IOSLoggedOutHomePage AssertHelpIconOnPage()
        {
            HelpIcon.AssertVisible();
            return this;
        }

        internal void ClickCovidBanner()
        {
            CovidConditions.Click();
        }

        internal void ClickHelpIcon()
        {
            HelpIcon.Click();
        }

        public void ContinueWithNhsLogin()
        {
            ContinueButton.Click();
        }
    }
}
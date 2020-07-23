using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSBanner CovidConditions => IOSBanner.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");

        private IOSLabel HowAreYouFeelingText => IOSLabel.WithText(_driver, "How are you feeling today?");
        private IOSLabel AccessServicesText => IOSLabel.WithText(_driver, "To access your NHS services");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue with NHS login");
        private IOSButton CheckYourSymptoms => new IOSButton(_driver, "Check symptoms");

        private IOSIcon HelpIcon => new IOSIcon(_driver, "NHS App help icon");


        public static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.HowAreYouFeelingText.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            CovidConditions.AssertVisible();
            AccessServicesText.AssertVisible();
            ContinueButton.AssertVisible();
            CheckYourSymptoms.AssertVisible();
            HelpIcon.AssertVisible();
        }

        public void GetInformationAboutCoronavirus() => CovidConditions.Click();

        public void GetHelp() => HelpIcon.Click();

        public void ContinueWithNhsLogin() => ContinueButton.Click();
    }
}
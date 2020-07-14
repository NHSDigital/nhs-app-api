using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel CovidConditions => IOSLabel.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");

        private IOSLabel HowAreYouFeelingText => IOSLabel.WithText(_driver, "How are you feeling today?");
        private IOSLabel AccessServicesText => IOSLabel.WithText(_driver, "To access your NHS services");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue with NHS login");
        private IOSButton CheckYourSymptoms => new IOSButton(_driver, "Check symptoms");

        private IOSIcon HelpIcon => new IOSIcon(_driver, "NHS app help icon");


        internal static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.HowAreYouFeelingText.AssertVisible();
            return page;
        }

        internal void AssertPageElements()
        {
            CovidConditions.AssertVisible();
            AccessServicesText.AssertVisible();
            ContinueButton.AssertVisible();
            CheckYourSymptoms.AssertVisible();
            HelpIcon.AssertVisible();
        }

        internal void GetInformationAboutCoronavirus() => CovidConditions.Click();

        internal void GetHelp() => HelpIcon.Click();

        internal void ContinueWithNhsLogin() => ContinueButton.Click();
    }
}
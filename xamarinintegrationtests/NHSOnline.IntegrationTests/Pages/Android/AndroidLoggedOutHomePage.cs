using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel CovidConditions => AndroidLabel.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");
        private AndroidLabel HowAreYouFeelingText => AndroidLabel.WithText(_driver, "How are you feeling today?");
        private AndroidLabel AccessServicesText => AndroidLabel.WithText(_driver, "To access your NHS services");

        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue with NHS login");
        private AndroidButton CheckYourSymptoms => new AndroidButton(_driver, "Check symptoms");
        private AndroidIcon HelpIcon => new AndroidIcon(_driver, "NHS app help icon");

        internal static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
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

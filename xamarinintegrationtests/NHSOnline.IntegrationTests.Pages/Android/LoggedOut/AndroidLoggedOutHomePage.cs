using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidBanner CovidConditions => AndroidBanner.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");
        private AndroidLabel HowAreYouFeelingText => AndroidLabel.WithText(_driver, "How are you feeling today?");
        private AndroidLabel AccessServicesText => AndroidLabel.WithText(_driver, "To access your NHS services");

        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue with NHS login");
        private AndroidButton CheckYourSymptoms => new AndroidButton(_driver, "Check symptoms");
        private AndroidIcon HelpIcon => new AndroidIcon(_driver, "NHS App help icon");

        public static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
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

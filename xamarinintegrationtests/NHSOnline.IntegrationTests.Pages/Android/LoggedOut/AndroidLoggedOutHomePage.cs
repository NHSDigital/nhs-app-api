using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidBanner CovidConditions => AndroidBanner.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");
        private AndroidLabel AccessServicesText => AndroidLabel.WithText(_driver, "To access your NHS services");

        private AndroidButton ContinueButton => AndroidButton.WithText(_driver, "Continue with NHS login");
        private AndroidIcon HelpIcon => AndroidIcon.WithDescription(_driver, "NHS App help icon");

        public static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
            page.AccessServicesText.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            CovidConditions.AssertVisible();
            AccessServicesText.AssertVisible();
            ContinueButton.AssertVisible();
            HelpIcon.AssertVisible();
        }

        public void AssertCovidBannerPresent()
        {
            CovidConditions.AssertVisible();
        }

        public void GetInformationAboutCoronavirus() => CovidConditions.Click();

        public void GetHelp() => HelpIcon.Click();

        public void ContinueWithNhsLogin() => ContinueButton.Click();
    }
}

using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSBanner CovidConditions => IOSBanner.WithText(_driver, "Coronavirus (COVID-19) Get information about coronavirus on NHS.UK");

        private IOSLabel AccessServicesText => IOSLabel.WithText(_driver, "Access your NHS services");

        private IOSLink ContinueButton => IOSLink.WithText(_driver, "Continue with NHS login");

        private IOSIcon HelpIcon => IOSIcon.WithDescription(_driver, "NHS App help icon");


        public static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
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

        public void AssertHelpIconPresent()
        {
            HelpIcon.AssertVisible();
        }

        public void GetInformationAboutCoronavirus() => CovidConditions.Click();

        public void GetHelp() => HelpIcon.Click();

        public void ContinueWithNhsLogin() => ContinueButton.Touch();
    }
}
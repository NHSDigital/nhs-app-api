using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel HowAreYouFeelingText => new AndroidLabel(_driver, "How are you feeling today?");
        private AndroidLabel AccessServicesText => new AndroidLabel(_driver, "To access your NHS services");

        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue with NHS login");
        private AndroidButton CheckYourSymptoms => new AndroidButton(_driver, "Check symptoms");

        internal static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
            page.HowAreYouFeelingText.AssertVisible();
            page.AccessServicesText.AssertVisible();
            page.CheckYourSymptoms.AssertVisible();
            page.ContinueButton.AssertVisible();
            return page;
        }

        public void ContinueWithNhsLogin()
        {
            ContinueButton.Click();
        }
    }
}

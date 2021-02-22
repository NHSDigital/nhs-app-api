using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-09.1", "Invoking native back on Android on the logged out home screen closes the the app")]
    public class LoggedOutHomeScreenBackAndroidTests
    {
        [NhsAppAndroidTest]
        public void APatientCanUseTheBackButtonToCloseTheAppOnTheLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver);

            driver.AssertRunningInForeground();

            driver.PressBackButton();

            driver.AssertNotRunningInForeground();
        }
    }
}
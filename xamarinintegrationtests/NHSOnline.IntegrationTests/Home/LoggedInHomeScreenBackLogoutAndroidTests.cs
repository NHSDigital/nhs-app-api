using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-NB-01.2", "Invoking native back on Android on the logged in home screen shows the logout prompt and the user can log out")]
    public class LoggedInHomeScreenBackLogoutAndroidTests
    {
        [NhsAppAndroidTest]
        public void APatientPressesTheBackButtonOnTheHomeScreenAndCanThenLogOutAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLogoutPrompt
                .AssertDisplayed(driver)
                .Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}
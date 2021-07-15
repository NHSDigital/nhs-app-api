using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-NB-01.1", "Invoking native back on Android on the logged in home screen shows the logout prompt")]
    [BusinessRule("BR-NB-01.3", "Invoking native back on Android on the logged in home screen shows the logout prompt and the user can cancel the prompt")]
    public class LoggedInHomeScreenBackCancelPromptAndroidTests
    {
        [NhsAppAndroidTest]
        public void APatientPressesTheBackButtonOnTheHomeScreenAndCanCancelToRemainLoggedInAndroid(IAndroidDriverWrapper driver)
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
                .Cancel();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}
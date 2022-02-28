using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookLoggedInHomeTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(
            ParentJourney = "A user logs into the app - Android",
            FlipbookTestName = "A user goes from home to advice and back")]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateFromHomeScreenAndGoBackHomeAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .KeyboardNavigateToAdvice(patient);

            AndroidAdvicePage
                .AssertOnPage(driver, screenshot: true)
                .KeyboardNavigateToHome();

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}
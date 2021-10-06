using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-NAV-01.1", "Clicking the home icon in the in the native header navigates a P9 user to the logged in homepage")]
    public class HomeTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateFromHomeScreenAndGoBackHomeAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAdvice(patient);

            AndroidAdvicePage
                .AssertOnPage(driver)
                .KeyboardNavigateToHome();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}
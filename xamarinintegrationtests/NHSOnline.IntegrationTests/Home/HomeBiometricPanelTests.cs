using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-HOM-01.1", "Biometrics call out is displayed when no biometrics preference has been set on a compatible device")]
    [BusinessRule("BR-HOM-01.2", "Biometrics call out is not displayed when it has been previously dismissed")]
    [BusinessRule("BR-HOM-01.3", "Biometrics call out is not displayed when the device is not compatible")]
    public class HomeBiometricPanelTests
    {
        [NhsAppAndroidTest]
        public void APatientThatHasNotSubmittedBiometricDecisionSeesBiometricPanelOnHomePageButNotAfterItHasBeenDismissedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Bjorn").FamilyName("Metrik"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelVisible()
                .DismissBiometricPanel();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelNotVisible();
        }

        [NhsAppIOSTest]
        public void APatientThatHasNotSubmittedBiometricDecisionSeesBiometricPanelOnHomePageButNotAfterItHasBeenDismissedIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Bjorn").FamilyName("Metrik"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelVisible()
                .DismissBiometricPanel();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelNotVisible();
        }

        [NhsAppManualTest("NHSO-15687", "Unable to test with a device below Android 6")]
        public void APatientThatHasADeviceNotCapableOfBiometricsDoesNotSeeTheBiometricPanelOnHomePageAndroid() { }
    }
}
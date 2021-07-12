using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-12.10", "Denying permissions when prompted dismisses the prompt when attempting to open the device camera and the user cannot continue until permissions are granted")]
    public class P5TakePhotoPermissionsRefusedTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveDenysPermissionsToTakeAPhotoOfTheirDocumentAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .OpenCamera();

            AndroidCameraPermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .AssertPhotoNotCaptured();
        }

        [NhsAppManualTest("NHSO-14221", "Denying permissions in the stub does not disable camera access")]
        public void APatientWithProofLevelFiveDenysPermissionsToTakeAPhotoOfTheirDocumentIos() { }
    }
}
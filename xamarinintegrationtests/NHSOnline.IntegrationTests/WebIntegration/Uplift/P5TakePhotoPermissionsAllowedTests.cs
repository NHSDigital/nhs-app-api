using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-12.9", "Choosing to take a photo an ID prompts the user to grant appropriate permissions to the app")]
    [BusinessRule("BR-LOG-12.11", "Allowing permissions when prompted opens the device camera enabling the user to capture a photo of their ID")]
    [BusinessRule("BR-LOG-12.12", "Capturing a photo of an ID prompts the user to confirm the attached photograph and upload it")]
    public class P5TakePhotoPermissionsAllowedTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanTakeAPhotoOfTheirDocumentAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5();
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
                .Allow();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidLocationPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidCamera
                .AssertDisplayed(driver)
                .TakePhoto();

            AndroidCamera
                .AssertDisplayed(driver)
                .Done();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .AssertPhotoCaptured();
        }

        [NhsAppIOSTest]
        [NhsAppFlakyTest]
        public void APatientWithProofLevelFiveCanTakeAPhotoOfTheirDocumentIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                 .AssertOnPage(driver)
                 .OpenCamera();

            IOSFileSourceDialog
                .IfDisplayed(driver, page => page.TakePhoto());

            IOSCameraPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSCamera
                .AssertDisplayed(driver)
                .Capture()
                .UsePhoto();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .AssertPhotoCaptured();
        }
    }
}
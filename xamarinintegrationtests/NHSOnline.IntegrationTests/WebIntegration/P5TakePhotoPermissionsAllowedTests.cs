using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("TBC", "Taking a photo of their ID allows the user to attach an image of the ID")]
    public class P5TakePhotoPermissionsAllowedTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanTakeAPhotoOfTheirDocumentAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.OpenCamera();

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
                .PageContent.AssertPhotoCaptured();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanTakeAPhotoOfTheirDocumentIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                 .AssertOnPage(driver)
                 .PageContent.OpenCamera();

            IOSImageSourceDialog
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
                .PageContent.AssertPhotoCaptured();
        }
    }
}
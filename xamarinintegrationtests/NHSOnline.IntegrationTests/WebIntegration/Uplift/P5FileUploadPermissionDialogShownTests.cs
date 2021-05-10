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
    [BusinessRule("BR-LOG-12.5", "Uploading a file in the NHS login uplift journey when the user has not granted appropriate permissions displays a native alert")]
    public class P5FileUploadPermissionDialogShown
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveUploadingAFileIsShownPermissionsDialogAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.UploadFile();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver);
        }


        [NhsAppManualTest("NHSO-13697", "BrowserStack does not show the permissions dialog for iOS")]
        public void APatientWithProofLevelFiveUploadingAFileIsShownPermissionsDialogIOS() { }

    }
}
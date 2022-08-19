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
    [BusinessRule("BR-LOG-12.4", "Uploading a file in the NHS login uplift journey allows the user to attach an image of their identification")]
    [BusinessRule("BR-LOG-12.6", "Uploading a file allows the user to attach an image of their ID ")]
    public class P5FileUploadPermissionsAllowedTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanUploadAFileAndroid(IAndroidDriverWrapper driver)
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
                .UploadFile();

            AndroidStorageProminentDialog
                .AssertDisplayed(driver)
                .Ok();

            AndroidFilePermissionsDialog
                .ClickAllowIfPresent(driver);

            AndroidFileChooser
                .AssertDisplayed(driver)
                .HamburgerClick()
                .ImagesMenuLabelClick()
                .PicturesLabelClick()
                .AssertThumbnailDisplayed()
                .ChoosePhoto();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .AssertFileSelected();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanUploadAFileIOS(IIOSDriverWrapper driver)
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
                .AssertNoFileSelected();

             IOSStubbedLoginUpliftPage
                 .AssertOnPage(driver)
                 .UploadFile();

             IOSFileSourceDialog
                 .GetPanel(driver)
                 .SelectPhotoLibrary();

            IOSFileChooser
                .AssertDisplayed(driver)
                .ChoosePhoto()
                .ConfirmSelection();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .AssertFileSelected();
        }
    }
}
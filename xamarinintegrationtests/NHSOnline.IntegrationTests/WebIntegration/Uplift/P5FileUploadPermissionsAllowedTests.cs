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
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileChooser
                .AssertDisplayed(driver)
                .ClickPictures()
                .AssertThumbnailDisplayed()
                .ChoosePhoto();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.AssertFileSelected();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanUploadAFileIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.AssertNoFileSelected();

             IOSStubbedLoginUpliftPage
                 .AssertOnPage(driver)
                 .UploadFile();

             IOSFileSourceDialog
                 .GetPanel(driver)
                 .SelectPhotoLibrary();

            IOSFileChooser
                .AssertDisplayed(driver)
                .SelectFolder()
                .ChoosePhoto()
                .ConfirmSelection();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.AssertFileSelected();
        }
    }
}
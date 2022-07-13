using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-WI-01.5",
        "Choosing to upload a file in a web integration opens the system file picker for the user to upload a file")]
    public class FileUploadWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToAWebIntegrationFileUploadScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            UploadAndVerifyFile(driver);

            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToFileUpload();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent
                .AssertFileNotSelected()
                .UploadFile();

            AndroidStoragePage
                .AssertOnPage(driver)
                .SelectFile();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();
        }

        // Running on lower version as we had issues with the structure in the updated default
        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone13, OSVersion = IOSVersion.Fifteen)]
        [NhsAppFlakyTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToAWebIntegrationFileUploadScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToFileUpload();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.UploadFile();

            IOSFileSourceDialog
                .GetPanel(driver)
                .SelectPhotoLibrary();

            IOSFileChooser
                .AssertDisplayed(driver)
                .ChoosePhoto()
                .ConfirmSelection();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();

        }

        private static void UploadAndVerifyFile(INativeDriverWrapper driver)
        {
            driver.PushTestFile();

            if (!driver.VerifyFilePushed())
            {
                Assert.Fail("Test could not find test file for upload");
            }
        }
    }
}
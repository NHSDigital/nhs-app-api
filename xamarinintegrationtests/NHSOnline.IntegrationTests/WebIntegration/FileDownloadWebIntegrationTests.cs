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
    public class FileDownloadWebIntegrationTests
    {
        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Ten)]
        public void APatientWithProofLevelNineCanDownloadAFileFromAWebIntegrationDownloadFileScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Nine)]
        public void APatientWithProofLevelNineCanDownloadAFileFromAWebIntegrationDownloadFileScreenNotUsingTheNewerOsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanDownloadAFileFromAWebIntegrationDownloadFileScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsIOS(driver);

            IOSFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            IOSShareFilePanel
                .AssertDisplayed(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanDenyPermissionsToDownloadAFileFromAWebIntegrationDownloadFileScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            AndroidFileDownloadPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanDownloadAPassKitFileFromAWebIntegrationDownloadFileScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsIOS(driver);

            IOSFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadPass();

            IOSPassKitController
                .AssertDisplayed(driver);
        }

        private static void NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(IAndroidDriverWrapper driver)
        {

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
                .NavigateToDocumentDownload();
        }

        private static void NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsIOS(IIOSDriverWrapper driver)
        {
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
                .NavigateToDocumentDownload();
        }
    }
}
using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Logs;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-WI-01.9",
        "Downloading a document when permissions have not been granted on the device prompts the user to grant permissions")]
    [BusinessRule("BR-WI-01.10",
        "Granting permissions on a device when downloading a document allows the document to be downloaded to the users chosen location on the device")]
    [BusinessRule("BR-WI-01.11",
        "Rejecting permissions on a device when downloading a document cancels the download action")]
    [BusinessRule("BR-WI-01.12",
        "Downloading a document when permissions have been granted on device downloads the document to the users chosen location on the device")]
    [BusinessRule("BR-WI-01.16",
        "Adding an pass to Apple wallet downloads the pass and add it to the users Apple Wallet")]
    public class FileDownloadWebIntegrationTests
    {
        private const string ThirdPartyHelpLinkPath = "health-records-in-the-nhs-app/third-party-services/";

        private const string DocumentDownloadHelpLinkPath = "health-records-in-the-nhs-app/gp-health-record/";

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
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadImage();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);

            AndroidFileDownloadPage? downloadPage = null;

            // additional steps to verify re-download does not fail
            TransitoryErrorHandler.HandleSpecificFailure()
                .Retry(() =>
                    {
                        driver.PressBackButton();

                        downloadPage = AndroidFileDownloadPage
                            .AssertOnPage(driver)
                            .AssertNativeHeaderAllIcons();
                    },
                    "No AndroidElement found matching By.XPath: .//android.view.ViewGroup[normalize-space(@content-desc)='NHS App Home']");

            downloadPage?
                .PageContent.DownloadImage();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Nine)]
        [NhsAppFlakyTest]
        public void
            APatientWithProofLevelNineCanDownloadAFileFromAWebIntegrationDownloadFileScreenNotUsingTheNewerOsAndroid(
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadImage();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);

            // additional steps to verify re-download does not fail
            driver.PressBackButton();

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadImage();

            AndroidGooglePhotosApp
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPadAir4, OSVersion = IOSVersion.Fourteen)]
        public void APatientWithProofLevelNineCanDownloadAFileFromAWebIntegrationDownloadFileScreenIpadIOS(
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
        public void
            APatientWithProofLevelNineCanDenyPermissionsToDownloadAFileFromAWebIntegrationDownloadFileScreenAndroid(
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
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

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedPassKitFileFromAWebIntegrationDownloadFileScreenAndCanTryAgainIOS(
                IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsIOS(driver);

            var timing = TimedTestExecutor.Execute(() =>
            {
                IOSFileDownloadPage
                    .AssertOnPage(driver)
                    .AssertNativeHeader()
                    .PageContent.DownloadCorruptedPass();

                IOSFileDownloadErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .TryAgain();

                IOSFileDownloadPage
                    .AssertOnPage(driver);

                // Wait for the asynchronous log call to be made
                Thread.Sleep(TimeSpan.FromSeconds(5));
            });

            var clientLogs = new ClientLoggerLogs(timing.StartTime, timing.StopTime);
            clientLogs.AssertClientLogStartsWith("Failed to construct a PKPass object as the data received is not in the expected format. DownloadRequest file length:");
        }

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedPassKitFileFromAWebIntegrationDownloadFileScreenAndGetDocumentDownloadHelpIOS(
                IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsIOS(driver);

            var timing = TimedTestExecutor.Execute(() =>
            {
                IOSFileDownloadPage
                    .AssertOnPage(driver)
                    .AssertNativeHeader()
                    .PageContent.DownloadCorruptedPass();

                IOSFileDownloadErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .GetDocumentDownloadHelp();

                IOSFileDownloadPage
                    .AssertOnPage(driver);

                // Wait for the asynchronous log call to be made
                Thread.Sleep(TimeSpan.FromSeconds(5));
            });

            var clientLogs = new ClientLoggerLogs(timing.StartTime, timing.StopTime);
            clientLogs.AssertClientLogStartsWith("Failed to construct a PKPass object as the data received is not in the expected format. DownloadRequest file length:");
        }

        [NhsAppAndroidTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingAPassKitFileFromAWebIntegrationDownloadFileScreenAndCanTryAgainAndroid
            (
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);
            LoginProcess.LogAndroidPatientIn(driver, patient);
            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadPass();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            AndroidFileDownloadPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingAPassKitFileFromAWebIntegrationDownloadFileScreenAndCanGetDocumentDownloadHelpAndroid
            (
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);
            LoginProcess.LogAndroidPatientIn(driver, patient);
            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadPass();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .GetDocumentDownloadHelp();

            AndroidFileDownloadPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanGetHelpAndroid
            (
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);
            LoginProcess.LogAndroidPatientIn(driver, patient);
            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadCorrupted();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Help();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, ThirdPartyHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppAndroidTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanGetDocumentDownloadHelpAndroid
            (
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);
            LoginProcess.LogAndroidPatientIn(driver, patient);
            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadCorrupted();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .GetDocumentDownloadHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, DocumentDownloadHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppAndroidTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanNavigateToAppointmentsAndroid
            (
                IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);
            LoginProcess.LogAndroidPatientIn(driver, patient);
            NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(driver);

            AndroidFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeaderAllIcons()
                .PageContent.DownloadCorrupted();

            AndroidFilePermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();

            driver.PressBackButton();

            AndroidFileDownloadPage
                .AssertOnPage(driver);

        }

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanTryAgainIOS
            (
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
                .PageContent.DownloadCorrupted();

            IOSFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            IOSFileDownloadPage
                .AssertOnPage(driver)
                .PageContent.DownloadCorrupted();

            IOSFileDownloadErrorPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSFileDownloadPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanGetHelpIOS
            (
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
                .PageContent.DownloadCorrupted();

            IOSFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Help();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, ThirdPartyHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanGetDocumentDownloadHelpIOS
            (
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
                .PageContent.DownloadCorrupted();

            IOSFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .GetDocumentDownloadHelp();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, DocumentDownloadHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void
            APatientWithProofLevelNineWillBeShownARelevantErrorScreenWhenDownloadingACorruptedFileFromAWebIntegrationDownloadFileScreenAndCanNavigateToAppointmentsIOS
            (
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
                .PageContent.DownloadCorrupted();

            IOSFileDownloadErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }

        private static void NavigateToAddToDocumentDownloadViaPkbHospitalAppointmentsAndroid(
            IAndroidDriverWrapper driver)
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
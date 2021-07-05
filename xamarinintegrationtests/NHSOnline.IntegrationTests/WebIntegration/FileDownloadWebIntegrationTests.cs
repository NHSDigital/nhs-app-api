using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class FileDownloadWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanDownloadAFileFromTheTestProviderDownloadFileScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateDownloadFile();

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
        public void APatientWithProofLevelNineCanDownloadAFileFromTheTestProviderDownloadFileScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateDownloadFile();

            IOSFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            IOSFilesApp
                .AssertDisplayed(driver)
                .SelectSaveImage();

            IOSPhotosPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            driver.BackgroundApp();

            driver.PressHome();

            driver.SwipeToNextScreen();

            IOSHomeScreen
                .AssertDisplayed(driver)
                .SelectPhotosApplication();

            IOSPhotosApp
                .AssertDisplayed(driver)
                .SelectPhotosTab()
                .AssertPhotoVisible();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanDenyPermissionsToDownloadAFileFromTheTestProviderDownloadFileScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateDownloadFile();

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
        public void APatientWithProofLevelNineCanDenyPermissionToDownloadAFileFromTheTestProviderDownloadFileScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateDownloadFile();

            IOSFileDownloadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.DownloadImage();

            IOSFilesApp
                .AssertDisplayed(driver)
                .SelectSaveImage();

            IOSPhotosPermissionDialog
                .AssertDisplayed(driver)
                .Deny();

            IOSFileDownloadPage
                .AssertOnPage(driver);
        }
    }
}
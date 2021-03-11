using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-12.7", "Rejecting the NHS app request for appropriate permissions in the NHS login uplift journey for file upload dismisses the native alert")]
    public class P5FileUploadPermissionRefusedTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveRefusesStoragePermissionAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent.Continue();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.UploadFile();

            AndroidPermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            /*
             Permissions dialog just closed on DENY,
             Browser stack does not reshow the dialog when the button is re-clicked.
             */
            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }


        [NhsAppManualTest("NHSO-13697", "BrowserStack does not show the permissions dialog for iOS")]
        public void APatientWithProofLevelFiveRefusesStoragePermissionIOS() { }

    }
}
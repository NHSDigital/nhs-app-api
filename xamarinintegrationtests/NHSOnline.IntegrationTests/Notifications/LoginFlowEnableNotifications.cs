using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Home.Biometrics;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-03.2", "Log in for a user who does not have notifications enabled and they have not seen the notifications prompt before displays the notification prompt")]
    [BusinessRule("BR-NOT-03.4", "Log in for the first time on a device displays the notification prompt")]
    public class LoginFlowEnableNotifications
    {
        [NhsAppAndroidTest]
        public void APatientCanEnableNotificationsDuringTheLoginFlowAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Jack").FamilyName("Potts"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
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
                .PageContent
                .AssertPageContent()
                .YesTurnOnNotifications()
                .Continue();

            AndroidFingerprintFaceIrisPromptPage
                .AssertOnPage(driver)
                .PageContent
                .NoTurnOffBiometrics()
                .Continue();

            AndroidSessionExpiryPrompt.ExtendIfDisplayed(driver);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent.AssertNotificationsEnabled();
        }

        [NhsAppManualTest("NHSO-14095", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientCanEnableNotificationsDuringTheLoginFlowIos() { }
    }
}
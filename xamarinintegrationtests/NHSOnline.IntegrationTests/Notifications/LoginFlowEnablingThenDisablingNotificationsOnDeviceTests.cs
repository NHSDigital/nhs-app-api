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
    [BusinessRule("BR-NOT-04.23", "If user attempts to turn on notifications when permission is denied, error is shown")]
    [BusinessRule("BR-NOT-04.24", "When clicking link provided in error message, settings opens on device")]
    public class LoginFlowEnablingThenDisablingNotificationsOnDeviceTests
    {
        [NhsAppAndroidTest]
        public void APatientEnablingNotificationsEnablesThenDisablesNotificationsOnTheDeviceAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Horace").FamilyName("Scope"));
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
                .PageContent.OpenNotificationSettings();

            AndroidSettingsAppInfo
                .AssertOnPage(driver)
                .NavigateToNotifications();

            AndroidSettingsNotifications
                .AssertOnPage(driver)
                .TurnOffNotifications();

            var androidSettingsNotifications = AndroidSettingsNotifications
                .AssertOnPage(driver);

            TransitoryErrorHandler.HandleSpecificFailure()
                .Alternate(() =>
                    {
                        // Intermittently there is no 'Back' arrow on this system notifications screen. In this case, just use native Back action.
                        androidSettingsNotifications
                            .ClickBack();
                    },
                    "No AndroidElement found matching By.XPath: //android.widget.ImageButton[normalize-space(@content-desc)='Navigate up']",
                    () => { driver.PressBackButton(); });

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent
                .ToggleOffNotificationsAndDoNotAssert();

            AndroidNotificationsPage
                .AssertCannotChangeNotificationsChoiceErrorOnPage(driver)
                .PageContent
                .AssertNotificationsChoiceErrorOnPageElements()
                .ClickDeviceSettings();

            AndroidSettingsAppInfo
                .AssertOnPage(driver);
        }

        [NhsAppManualTest("NHSO-18578", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientEnablingNotificationsEnablesThenDisablesNotificationsOnTheDeviceIOS() { }
    }
}
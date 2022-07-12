using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.9",
        "Navigating to notification settings when notifications have been disabled for the device in the device settings displays an error message")]
    public class NavigateToNotificationsWhenDisabledOnDeviceTests
    {
        [NhsAppAndroidTest]
        public void APatientNavigatesToNotificationsWhenDisabledOnDeviceAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Sandy").FamilyName("Beech"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

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
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertNotificationsTurnedOffOnDeviceErrorOnPage(driver)
                .PageContent.AssertNotificationsTurnedOffErrorPageElements();
        }

        [NhsAppManualTest("NHSO-14101",
            "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientNavigatesToNotificationsWhenDisabledOnDeviceIOS()
        {
        }
    }
}
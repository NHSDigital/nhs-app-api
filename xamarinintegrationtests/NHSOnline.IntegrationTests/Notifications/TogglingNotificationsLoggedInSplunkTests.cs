using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Logs;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.15", "Enabling or disabling notifications settings on a device writes appropriate logs in Splunk for analytic purposes")]
    public class TogglingNotificationsLoggedInSplunkTests
    {
        [NhsAppAndroidTest]
        public void APatientEnablesNotificationsSettingsOnADeviceWritesLogToSplunkForAnalyticsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Spur").FamilyName("Lincoln"));
            using var patients = Mocks.Patients.Add(patient);

            var testTiming = TimedTestExecutor.Execute(() => LoginProcess.LogAndroidPatientIn(driver, patient));

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent
                .ToggleOnNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHome();

            NotificationLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "users",
                    patient,
                    patient.OdsCode,
                    "NotificationsEnabled")
                .AssertFound();
        }

        [NhsAppAndroidTest]
        public void APatientDisablesNotificationsSettingsOnADeviceWritesLogToSplunkForAnalyticsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Spur").FamilyName("Lincoln"));
            using var patients = Mocks.Patients.Add(patient);

            var testTiming = TimedTestExecutor.Execute(() => LoginProcess.LogAndroidPatientIn(driver, patient));

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent
                .ToggleOnNotifications()
                .ToggleOffNotifications();

            NotificationLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "users",
                    patient,
                    patient.OdsCode,
                    "NotificationsDisabled")
                .AssertFound();
        }

        [NhsAppManualTest("NHSO-14110", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientEnablesOrDisablesNotificationsSettingsOnADeviceWritesLogToSplunkForAnalyticsIOS() { }
    }
}
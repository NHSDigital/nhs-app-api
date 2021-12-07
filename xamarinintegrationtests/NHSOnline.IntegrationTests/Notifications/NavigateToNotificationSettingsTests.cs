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
    [BusinessRule("BR-NOT-04.22", "Actioning the manage notifications settings link from the notifications settings menu navigates to the NHS App device settings")]
    public class NavigateToNotificationSettingsTests
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToDeviceSettingsFromNotificationsPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Richard").FamilyName("Headley"));
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
                .AssertPageElements()
                .PageContent.OpenNotificationSettings();

            AndroidSettingsAppInfo
                .AssertOnPage(driver)
                .NavigateToNotifications();

            AndroidSettingsNotifications
                .AssertOnPage(driver)
                .AssertPageContent();
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToDeviceSettingsFromHelpAndSupportViaKeyboardAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Seymour").FamilyName("Buttons"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore(patient);

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToDeviceSettings();

            AndroidSettingsAppInfo
                .AssertOnPage(driver);
        }
    }
}
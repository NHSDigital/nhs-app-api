using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-LOG-04.22", "Actioning the manage notifications settings from the notifications settings menu navigates to the device settings")]
    public class NavigateToNotificationSettings
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToDeviceSettingsFromNotificationsPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Richard").FamilyName("Headley"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
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
                .AssertPageContent();
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToDeviceSettingsFromNotificationsViaKeyboardAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Seymour").FamilyName("Buttons"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToDeviceSettings();

            AndroidSettingsAppInfo
                .AssertOnPage(driver);
        }
    }
}
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
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Richard").FamilyName("Head"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

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
                .AssertPageContent()
                .NavigateBack();

            AndroidNotificationsPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToDeviceSettingsFromNotificationsViaKeyboardAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Seymour").FamilyName("Butts"));
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
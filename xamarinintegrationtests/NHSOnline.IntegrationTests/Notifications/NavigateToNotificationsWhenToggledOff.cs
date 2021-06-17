using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.2", "Navigating to manage notifications from the settings menu when the notifications for the device are disabled displays the manage notifications screen with the current registration status for the device toggled to off")]
    public class NavigateToNotificationsWhenToggledOff
    {
        [NhsAppAndroidTest]
        public void APatientNavigatesToNotificationsWhenToggledOffAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Stan").FamilyName("Dupp"));
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
                .PageContent
                .ToggleOnNotifications()
                .ToggleOffNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent.AssertNotificationsDisabled();
        }

        [NhsAppManualTest("NHSO-14096", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientNavigatesToNotificationsWhenToggledOffIOS() { }
    }
}
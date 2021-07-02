using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.8", "Disabling notifications successfully disables notifications on the device")]
    public class DisablingNotificationsWhenEnabledTests
    {
        [NhsAppAndroidTest]
        public void APatientDisablesNotificationsWhenAlreadyEnabledAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Robyn").FamilyName("Banks"));
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
                .AssertNotificationsEnabled();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent
                .ToggleOffNotifications()
                .AssertNotificationsDisabled();
        }

        [NhsAppManualTest("NHSO-14100", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientDisablesNotificationsWhenAlreadyEnabledIOS() { }
    }
}
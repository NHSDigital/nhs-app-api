using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    public class LoggedInNotificationsTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessNotificationsFromTheMorePage(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessNotificationsFromTheMorePageAndBothRegisterAndDeregisterForNotifications(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent.ToggleOnNotifications()
                .ToggleOffNotifications();
        }

        [NhsAppAndroidTest]
        public void APatientCanEnableNotificationsAndTheirDecisionPersists(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent.ToggleOnNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            AndroidNotificationsPage
                .AssertOnPage(driver)
                .PageContent.AssertNotificationsEnabled();
        }

        private static void LogAndroidPatientIn(IAndroidDriverWrapper driver, Patient patient)
        {
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
        }
    }
}
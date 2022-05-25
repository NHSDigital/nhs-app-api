using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.1", "Navigating to a service via the native footer the nav icon is highlighted")]
    public class CorrectIconIsSelectedOnClick
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToEachHubUsingTheBottomNavAndCanSeeTheRelevantBottomNavIconHighlightedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.AssertAppointmentsSelected();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();
        }

		[NhsAppFlakyTest]
        [NhsAppIOSTest]
        public void APatientCanNavigateToEachHubUsingTheBottomNavAndCanSeeTheRelevantBottomNavIconHighlightedIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.AssertAppointmentsSelected();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();
        }
    }
}
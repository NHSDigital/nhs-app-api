using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.5", "Navigating to another service via the footer deselects the currently highlighted icon")]
    public class NavigatingFromASelectedIconDeselectsTheCurrentlySelectedIconTests
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingFromOneHubToAnotherUsingTheBottomNavSeesThePreviousBottomNavIconBecomeUnhighlightedAndroid(
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
                .Navigation.AssertAppointmentsNotSelected();
        }

        [NhsAppIOSTest]
        public void APatientNavigatingFromOneHubToAnotherUsingTheBottomNavSeesThePreviousBottomNavIconBecomeUnhighlightedIOS(
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
                .Navigation.AssertAppointmentsNotSelected();
        }
    }
}
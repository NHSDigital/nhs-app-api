using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.5", "Navigating to another service via the footer deselects the currently highlighted icon")]
    public class NavigatingFromASelectedIconDeselectsTheCurrentlySelectedIconTest
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingFromOneHubToAnotherUsingTheBottomNavSeesThePreviousBottomNavIconBecomeUnhighlightedAndroidAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AppointmentsIcon.Click();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.AppointmentsIcon.AssertSelected();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.Click();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.AssertSelected();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AppointmentsIcon.AssertNotSelected();
        }
    }
}
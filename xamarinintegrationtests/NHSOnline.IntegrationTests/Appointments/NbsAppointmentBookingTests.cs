using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Appointments
{
    [TestClass]
    public class NbsAppointmentBookingTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlakyTest]
        public void APatientCanAccessTheirNbsAdditionalBookingFromAppointmentsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Nbs)
                .WithName(b => b.GivenName("Anne").FamilyName("Teak"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertNbsElements()
                .NavigateToNbsAdditionsBookings();

            AndroidNbsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();;
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirNbsAdditionalBookingFromAppointmentsScreenIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Nbs)
                .WithName(b => b.GivenName("Anne").FamilyName("Teak"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.AssertPageElements()
                .AssertNbsElements()
                .NavigateToNbsAdditionsBookings();

            IOSNbsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();;
        }
    }
}
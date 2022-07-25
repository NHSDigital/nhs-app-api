using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookBookAppointmentGpSupplierDown
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "A user accessing GP appointments when GP Supplier down")]
        public void APatientWithProofLevelNineCannotAccessGpAppointmentsWhenGpSupplierDownIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled).WithBehaviour(new EmisCreateSessionFailureBehaviour())
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);
            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToGpSurgeryAppointments();

            IOSSessionExpiryPrompt.ExtendIfDisplayed(driver);

            var iosAppointmentsTemporaryProblemPage = IOSAppointmentsTemporaryProblemPage
                .AssertOnPage(driver, screenshot: true);

            iosAppointmentsTemporaryProblemPage.TryAgain();

            var iosAppointmentBookingUnavailablePage = IOSAppointmentBookingUnavailablePage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}


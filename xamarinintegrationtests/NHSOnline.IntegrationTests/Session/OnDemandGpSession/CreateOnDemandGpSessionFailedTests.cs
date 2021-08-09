using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.OnDemandGpSession
{
    [TestClass]
    public class CreateOnDemandGpSessionFailedTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenThereIsAFailureCreatingAGpSessionOnDemandAndroid(IAndroidDriverWrapper driver)
        {
            var fo = new EmisPatient()
                .WithName(b => b.GivenName("Fo").FamilyName("Catcha"))
                .WithBehaviour(new NhsLoginReauthorizeSSOBehaviour());
            var brie = new TppPatient()
                .WithName(b => b.GivenName("Brie").FamilyName("Oche"));

            using var patients = Mocks.Patients.Add(fo, brie);

            LoginProcess.LogAndroidPatientIn(driver, fo);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToGpSurgeryAppointments();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(brie);

            AndroidAppointmentsTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            AndroidAppointmentBookingUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenThereIsAFailureCreatingAGpSessionOnDemandIOS(IIOSDriverWrapper driver)
        {
            var anna = new EmisPatient()
                .WithName(b => b.GivenName("Anna").FamilyName("Damma"))
                .WithBehaviour(new NhsLoginReauthorizeSSOBehaviour());
            var crew = new TppPatient()
                .WithName(b => b.GivenName("Crew").FamilyName("Tonn"));

            using var patients = Mocks.Patients.Add(anna, crew);

            LoginProcess.LogIOSPatientIn(driver, anna);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .GoToGpSurgeryAppointments();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(crew);

            IOSAppointmentsTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            IOSAppointmentBookingUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}
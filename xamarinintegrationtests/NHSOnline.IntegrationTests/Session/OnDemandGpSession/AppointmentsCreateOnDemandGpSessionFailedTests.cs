using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.OnDemandGpSession
{
    [TestClass]
    [BusinessRule("BR-GEN-04.6", "Failure to obtain a GP session on the first attempt in the user session displays a service specific 'try again' error to the user")]
    [BusinessRule("BR-GEN-04.8", "Failure to obtain a GP session when the user has initiated another attempt to get a GP session via a try again displays a specific service unavailable shutter page to the user")]
    public class AppointmentsCreateOnDemandGpSessionFailedTests
    {
        [NhsAppCanaryTest]
        [NhsAppAndroidTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenTryingToViewAppointmentsAndThereIsAFailureCreatingAGpSessionOnDemandAndCanReportTheProblemAndroid(IAndroidDriverWrapper driver)
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

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(brie);

            AndroidAppointmentsTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            AndroidAppointmentBookingUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .PageContent.ReportAProblem();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .AssertErrorCode("3c")
                .ReturnToApp();

            driver.PressBackButton();

            AndroidAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppCanaryTest]
        [NhsAppIOSTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenThereIsAFailureCreatingAGpSessionOnDemandAndCanReportTheProblemIOS(IIOSDriverWrapper driver)
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
                .AssertPageElements()
                .PageContent.ReportAProblem();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .AssertErrorCode("3c")
                .ReturnToApp();

            driver.SwipeBack();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .GoToGpSurgeryAppointments();
        }
    }
}
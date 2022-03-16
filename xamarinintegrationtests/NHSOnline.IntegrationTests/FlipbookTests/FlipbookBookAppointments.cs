using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookBookAppointmentTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - Android",
            FlipbookTestName = "A user books a GP appointment")]
        public void APatientWithProofLevelNineCanBookAnAppointmentAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .KeyboardNavigateToAppointments(patient);

            AndroidAppointmentsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToGpSurgeryAppointments();

            AndroidGpSurgeryAppointmentsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ClickBookAppointment();

            var androidBookAppointmentsPage = AndroidBookAppointmentsPage
                .AssertOnPage(driver, screenshot: true);

            androidBookAppointmentsPage.PageContent.ChooseType();
            androidBookAppointmentsPage.ClickType();
            androidBookAppointmentsPage.PageContent.Toggle();
            androidBookAppointmentsPage.ScrollAndScreenshot();
            androidBookAppointmentsPage.PageContent.ClickLink();

            var androidAppointmentConfirmPage = AndroidAppointmentConfirmPage
                .AssertOnPage(driver, screenshot: true);

            androidAppointmentConfirmPage.PageContent.InsertReason();
            androidAppointmentConfirmPage.ScrollAndScreenshot();
            androidAppointmentConfirmPage.PageContent.ClickBook();

            AndroidAppointmentConfirmedPage
                .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "A user books a GP appointment")]
        public void APatientWithProofLevelNineCanBookAnAppointmentIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
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

            var iosGpSurgeryAppointmentsPage = IOSGpSurgeryAppointmentsPage
                .AssertOnPage(driver, screenshot: true);

            iosGpSurgeryAppointmentsPage.PageContent.ClickBookAppointment();

            var iosBookAppointments = IOSBookAppointmentsPage
                .AssertOnPage(driver, screenshot: true);

            iosBookAppointments.PageContent.ChooseType();
            iosBookAppointments.SetPickerValue("Practice");
            iosBookAppointments.PageContent.Toggle();

            iosBookAppointments.ScrollAndScreenshot();
            iosBookAppointments.PageContent.ClickLink();

            var iosAppointmentConfirmPage = IOSAppointmentConfirmPage
                .AssertOnPage(driver, screenshot: true);

            iosAppointmentConfirmPage.PageContent.InsertReason();
            iosAppointmentConfirmPage.ScrollAndScreenshot();
            iosAppointmentConfirmPage.PageContent.ClickBook();

            IOSAppointmentConfirmedPage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}
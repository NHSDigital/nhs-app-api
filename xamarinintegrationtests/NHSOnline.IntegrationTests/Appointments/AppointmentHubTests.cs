using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Appointments
{
    [TestClass]
    [BusinessRule("BR-APT-02.1", "Show appointment hub page")]
    public class AppointmentHubTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownTheAppointmentHubPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("App").FamilyName("Ointments"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertAdditionalGpServicesElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheAppointmentHubPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("App").FamilyName("Ointments"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertAdditionalGpServicesElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownAnErrorWhenTryingToAccessTheSurgeryGpAppointmentsPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("App").FamilyName("Ointments"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .NavigateToGpSurgeryAppointments();

            IOSGpSurgeryAppointmentsPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientIsShownAppointmentsHubAndCanNavigateBackFromLinksViaKeyboardNavigationAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("App").FamilyName("Ointments"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAppointments(patient);

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToAdditionalGpServices();

            AndroidAdditionalGpServicesPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToGpSurgeryAppointments();

            AndroidGpSurgeryAppointmentsPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToGpSurgeryAppointments();

            AndroidGpSurgeryAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertEngageElements();
        }
    }
}
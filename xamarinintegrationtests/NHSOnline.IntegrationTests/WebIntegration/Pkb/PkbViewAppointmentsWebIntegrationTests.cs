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

namespace NHSOnline.IntegrationTests.WebIntegration.Pkb
{

    [TestClass]
    public class PkbViewAppointmentsWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbAppointmentsFromTheAppointmentsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Patrick").FamilyName("Klingon"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.HospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, "/diary/listAppointments.action")
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbAppointmentsFromTheAppointmentsScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Barry").FamilyName("Allen"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.HospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver,"/diary/listAppointments.action")
                .AssertNativeHeader();
        }
    }
}
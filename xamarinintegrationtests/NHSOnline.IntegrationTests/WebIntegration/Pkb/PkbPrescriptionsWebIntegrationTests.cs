using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Pkb
{

    [TestClass]
    public class PkbPrescriptionsWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbPrescriptionsFromThePrescriptionsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Arthur").FamilyName("Curry"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Prescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other prescriptions")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, "/auth/manageMedications.action?tab=treatments")
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbPrescriptionsFromThePrescriptionsScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Hal").FamilyName("Jordan"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Prescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other prescriptions")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, "/auth/manageMedications.action?tab=treatments")
                .AssertNativeHeader();
        }
    }
}
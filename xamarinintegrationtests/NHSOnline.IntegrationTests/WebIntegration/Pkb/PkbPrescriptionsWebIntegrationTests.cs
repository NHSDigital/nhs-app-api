using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
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
        [NhsAppFlakyTest]
        public void APatientCanAccessTheirPkbPrescriptionsFromThePrescriptionsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Arthur").FamilyName("Curry"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        [NhsAppFlakyTest]
        public void APatientCanAccessTheirPkbPrescriptionsFromThePrescriptionsScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Hal").FamilyName("Jordan"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines");

            driver.SwipeBack();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .AssertNativeHeader();
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-GEN-01.1", "An action that requires location services in a webview when location services have not been enabled for the native device displays a permissions prompt")]
    [BusinessRule("BR-GEN-01.2", "Granting permissions when prompted to enable location services from an action in a webview enables location services services for that device")]
    [BusinessRule("BR-GEN-01.3", "Rejecting permissions when prompted to enabled location services from an action in a webview results in the page in a webview not having access to location services")]
    [BusinessRule("BR-GEN-01.4", "An action that requires location services in a webview when location services have already been enabled on the device for the app gives access to location services in a webview")]
    public class LocationServicesWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNotAccessLocationServicesWithinTheAppWhenTheyDoNotAcceptPermissionsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToLocationServicesViaPkbHospitalAppointmentsAndroid(driver);

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ShowLocation();

            AndroidLocationProminentDialog
                .AssertDisplayed(driver)
                .Ok();

            AndroidLocationServicesPermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertErrorTextPresented();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessLocationServicesWithinTheAppWhenTheyAcceptPermissionsIos(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToLocationServicesViaPkbHospitalAppointmentsIOS(driver);

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.ShowLocation();

            IOSLocationServicesPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSLocationServicesSitePermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertLocationPresented();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNotAccessLocationServicesWithinTheAppWhenTheyDoNotAcceptPermissionsIos(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToLocationServicesViaPkbHospitalAppointmentsIOS(driver);

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.ShowLocation();

            IOSLocationServicesPermissionDialog
                .AssertDisplayed(driver)
                .Deny();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertErrorTextPresented();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessLocationServicesWithinTheAppWhenTheyAcceptPermissionsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToLocationServicesViaPkbHospitalAppointmentsAndroid(driver);

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ShowLocation();

            AndroidLocationProminentDialog
                .AssertDisplayed(driver)
                .Ok();

            AndroidLocationServicesPermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertLocationPresented();
        }

        private static void NavigateToAddToLocationServicesViaPkbHospitalAppointmentsAndroid(IAndroidDriverWrapper driver)
        {

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToLocationServices();
        }

        private static void NavigateToAddToLocationServicesViaPkbHospitalAppointmentsIOS(IIOSDriverWrapper driver)
        {
            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToLocationServices();
        }
    }
}
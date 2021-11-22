using AngleSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-WI-01.20", "Navigating to a link on a known service domain that has been specified to open in a browser overlay opens the requested page in a browser overlay")]
    public class OpenBrowserOverlayWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessAWebIntegrationAndCallOpenBrowserOverlayAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsAndroid(driver);

            AndroidOpenBrowserOverlayPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterOverlayURL(driver, new Url("https://www.nhs.uk/"))
                .clickOverlayButton(driver);

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlay
                .AssertInBrowserOverlay(driver)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAWebIntegrationAndCallOpenBrowserOverlayIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsIOS(driver);

            IOSOpenBrowserOverlayPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterOverlayURL(driver, new Url("https://www.nhs.uk/"))
                .clickOverlayButton(driver);

            IOSBrowserOverlay
                .AssertInBrowserOverlay(driver)
                .ReturnToApp();

        }

        private static void NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsAndroid(
            IAndroidDriverWrapper driver)
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
                .NavigateToOpenBrowserOverlay();
        }

        private static void NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsIOS(IIOSDriverWrapper driver)
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
                .NavigateToOpenBrowserOverlay();
        }
    }
}
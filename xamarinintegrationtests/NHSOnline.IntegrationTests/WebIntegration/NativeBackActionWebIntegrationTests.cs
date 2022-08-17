using System;
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
    public class NativeBackActionWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessAWebIntegrationAndCallNativeBackActionWithAFunctionAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToNativeBackActionViaPkbHospitalAppointmentsAndroid(driver);

            AndroidNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterNativeBackAction(driver, "window.location.href = \"http://stackoverflow.com\"")
                .ClickSetBackActionButton(driver)
                .ClickSetBackActionButton(driver)
                .PressBackButton(driver);

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlay
                .AssertInBrowserOverlay(driver)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAWebIntegrationAndCallNativeBackActionWithAFunctionIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsIOS(driver);

            IOSNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterNativeBackAction(driver, "window.location.href = \"http://stackoverflow.com\"")
                .ClickSetBackActionButton(driver)
                .SwipeBackAction(driver);

            IOSBrowserOverlay
                .AssertInBrowserOverlay(driver)
                .ReturnToApp();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessAWebIntegrationAndCallNativeBackActionWithoutAFunctionAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToNativeBackActionViaPkbHospitalAppointmentsAndroid(driver);

            AndroidNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ClickSetBackActionButton(driver)
                .PressBackButton(driver);

            AndroidNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAWebIntegrationAndCallNativeBackActionWithoutAFunctionIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToOpenBrowserOverlayViaPkbHospitalAppointmentsIOS(driver);

            IOSNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ClickSetBackActionButton(driver)
                .SwipeBackAction(driver);

            IOSNativeBackActionPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        private static void NavigateToNativeBackActionViaPkbHospitalAppointmentsAndroid(
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
                .NavigateToNativeBackAction();
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
                .NavigateToNativeBackAction();
        }
    }
}
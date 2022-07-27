using System;
using System.Threading;
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
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-WI-01.6", "Adding an appointment to calendar with valid start and end dates invokes native calendar functionality with relevant fields supplied")]
    [BusinessRule("BR-WI-01.7", "Adding an appointment to calendar when invalid params have been sent prompts the user to add an event manually to the calendar")]
    public class AddToCalendarWebIntegrationTests
    {
        private const decimal ValidStartTime = 1893589200.123m; // Wednesday, 2 January 2030 13:00:00.123
        private const decimal ValidEndTime = 1893589800.456m; // Wednesday, 2 January 2030 13:10:00.456
        private const decimal InvalidEndTime = 1893589100.789m; // Wednesday, 2 January 2030 12:58:20.789

        [NhsAppAndroidTest(AndroidBrowserStackCapability.SignInToGoogle)]
        public void APatientWithProofLevelNineCanAddAnEventToTheCalendarOnAWebIntegrationCalendarScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsAndroid(driver);

            var androidCalendarPage = AndroidCalendarPage
                .AssertOnPage(driver);

            androidCalendarPage
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .AddCalendarEvent();

            AndroidPlatformCalendarsApp
                .AssertOnPage(driver)
                .AssertDetailsArePassed();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAddAnEventToTheCalendarOnAWebIntegrationCalendarScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsIOS(driver);

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSCalendarsApp
                .AssertOnPage(driver)
                .AssertDetailsArePassed();
        }

        [NhsAppAndroidTest(AndroidBrowserStackCapability.SignInToGoogle)]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnAWebIntegrationCalendarScreenIsShownAndErrorDialogCanBeDismissedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsAndroid(driver);

            AndroidCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .AddCalendarEvent();

            AndroidCalendarErrorDialog
                .AssertDisplayed(driver)
                .Ok();

            AndroidCalendarPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnAWebIntegrationCalendarScreenIsShownAndErrorDialogCanBeDismissedIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsIOS(driver);

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarValidationErrorDialog
                .AssertDisplayed(driver)
                .Ok();

            IOSCalendarPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidBrowserStackCapability.SignInToGoogle)]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnAWebIntegrationCalendarScreenIsShownAnErrorDialogAndCanAddedManuallyAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsAndroid(driver);

            AndroidCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .AddCalendarEvent();

            AndroidCalendarErrorDialog
                .AssertDisplayed(driver)
                .AddEventManually();

            AndroidPlatformCalendarsApp
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnAWebIntegrationCalendarScreenIsShownAnErrorDialogAndCanAddedManuallyIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsIOS(driver);

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarValidationErrorDialog
                .AssertDisplayed(driver)
                .AddEventManually();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSCalendarsApp
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToAddToCalendarWithValidDetailsDeniesAccessAndIsShownTheRelevantWarningIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToAddToCalendarViaPkbHospitalAppointmentsIOS(driver);

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Deny();

            IOSCalendarPermissionErrorDialog
                .AssertDisplayed(driver);
        }

        private static void NavigateToAddToCalendarViaPkbHospitalAppointmentsAndroid(IAndroidDriverWrapper driver)
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
                .NavigateToCalendar();
        }

        private static void NavigateToAddToCalendarViaPkbHospitalAppointmentsIOS(IIOSDriverWrapper driver)
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
                .NavigateToCalendar();
        }
    }
}
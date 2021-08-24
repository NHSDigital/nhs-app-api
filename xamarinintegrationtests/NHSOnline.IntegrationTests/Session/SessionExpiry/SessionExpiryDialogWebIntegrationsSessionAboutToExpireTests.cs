using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.Session
{
    [Ignore("The functionality is broken but being resolved under NHSO-15938")]
    [TestClass]
    [BusinessRule("BR-LOG-07.2","User is prompted to logout or extend their session when their session reaches predefined time until it expires")]
    [BusinessRule("BR-LOG-07.7","Navigating back to the app from a web integration or browser overlay when the app session is within the session timeout warning period displays a prompt to extend the session or log out")]
    public class SessionExpiryDialogWebIntegrationsSessionAboutToExpireTests
    {
        private static readonly TimeSpan SessionExpiryDialogDuration = TimeSpan.FromMinutes(1);

        [NhsAppAndroidTest]
        public void APatientWhoAccessesAThirdPartyAndReturnsAsTheSessionIsAboutToExpireWillSeeTheExpiryPromptAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Teresa").FamilyName("Green"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

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
                .AssertNativeHeader();

            Thread.Sleep(SessionExpiryDialogDuration);

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .Navigation.NavigateToPrescriptions();

            AndroidSessionExpiryPrompt
                .AssertDisplayed(driver)
                .ExtendSession();

            AndroidPrescriptionsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWhoAccessesAThirdPartyAndReturnsAsTheSessionIsAboutToExpireWillSeeTheExpiryPromptIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Barry").FamilyName("Allen"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

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
                .AssertNativeHeader();

            Thread.Sleep(SessionExpiryDialogDuration);

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .Navigation.NavigateToAppointments();

            IOSSessionExpiryPrompt
                .AssertDisplayed(driver)
                .ExtendSession();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }
    }
}
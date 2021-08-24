using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.Session
{
    [Ignore("The functionality is broken but being resolved under NHSO-15938")]
    [TestClass]
    [BusinessRule("BR-LOG-07.6","Navigating back to the app from a web integration or browser overlay when the app session has expired logs the user out redirecting to the logged out home screen with a yellow banner")]
    public class SessionExpiredWebIntegrationsTests
    {
        private static readonly TimeSpan SessionExpiredDuration = TimeSpan.FromMinutes(2.5);

        [NhsAppAndroidTest(AndroidBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientWhoAccessesAThirdPartyAndReturnsWhenTheSessionHasExpiredWillSeeTheLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Patrick").FamilyName("Klingon"));
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

            Thread.Sleep(SessionExpiredDuration);

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .Navigation.NavigateToAppointments();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientWhoAccessesAThirdPartyAndReturnsWhenTheSessionHasExpiredWillSeeTheLoggedOutHomeScreenIOS(IIOSDriverWrapper driver)
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
                .AssertOnPage(driver,"/diary/listAppointments.action")
                .AssertNativeHeader();

            Thread.Sleep(SessionExpiredDuration);

            IOSPkbPage
                .AssertOnPage(driver,"/diary/listAppointments.action")
                .Navigation.NavigateToAppointments();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }
    }
}
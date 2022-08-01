using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [BusinessRule("BR-IC-01.2", "While within any journey that engages Web Integration, if Internet connectivity is lost on a device, error screen with back to home link is displayed which enables going back to Home view for the User")]
    [TestClass]
    public class WebIntegrationBackToHomeConnectionErrorTests
    {
        [NhsAppAndroidTest]
        [Ignore("Outstanding ticket (705318) with BS Support ignoring until resolution")]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorDuringWebIntegrationsAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Stevie").FamilyName("Kenarban"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .KeyboardNavigateToContinue();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments);

            await driver.EnableAirplaneMode();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .NavigateToGoToPage();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidGoToPage
                        .AssertOnPage(driver);
                });

            // For some reason this test consistently fails when there is no delay after resetting the network
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(5));

            AndroidBackToHomeConnectionErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidLoggedInHomePage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidInternetConnectionErrorPage
                        .AssertOnPage(driver)
                        .AssertPageElements();
                });
        }

        [NhsAppIOSTest]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorDuringWebIntegrationsIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Jamie").FamilyName("Wilkerson"));

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
                .AssertOnPage(driver, PhrPath.ViewAppointments);

            await driver.DisableNetwork();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .NavigateToGoToPage();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSGoToPage
                        .AssertOnPage(driver);
                });

            // For some reason this test consistently fails when there is no delay after resetting the network
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(5));

            IOSBackToHomeConnectionErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSLoggedInHomePage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSInternetConnectionErrorPage
                        .AssertOnPage(driver)
                        .AssertPageElements();
                });
        }
    }
}
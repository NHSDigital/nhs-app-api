using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [TestClass]
    public class OnDemandGpSessionBackToHomeConnectionErrorTests
    {
        [NhsAppAndroidTest]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorCreatingOnDemandGpSessionAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Piama").FamilyName("Wilkerson"))
                .WithBehaviour(new NhsLoginSlowResponseSSOBehaviour(TimeSpan.FromSeconds(60)));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            await Task.Delay(TimeSpan.FromSeconds(1));

            await driver.EnableAirplaneMode();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidMessagesPage
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
                        .AssertOnPage(driver);
                });
        }

        [NhsAppIOSTest]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorCreatingOnDemandGpSessionIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Malcolm").FamilyName("Wilkerson"))
                .WithBehaviour(new NhsLoginSlowResponseSSOBehaviour(TimeSpan.FromSeconds(60)));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            await Task.Delay(TimeSpan.FromSeconds(1));

            await driver.DisableNetwork();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSMessagesPage
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
                        .AssertOnPage(driver);
                });
        }
    }
}
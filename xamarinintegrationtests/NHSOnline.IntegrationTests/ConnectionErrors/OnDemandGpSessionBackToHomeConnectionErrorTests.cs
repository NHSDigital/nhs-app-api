using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [TestClass]
    [BusinessRule("BR-IC-01.4", "While within any journey that engages Web Integration, if Internet connectivity is lost on a device, error screen with back to home link is displayed which enables going back to Home view for the User")]
    public class OnDemandGpSessionBackToHomeConnectionErrorTests
    {
        [NhsAppFlakyTest]
        [NhsAppAndroidTest]
        [Ignore("Outstanding ticket (705318) with BS Support ignoring until resolution")]
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
                .GoTo111();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            // Need to reuse the same page instance in the known issue fallback assertion
            // as creating a new one will result in a new context being grabbed.
            AndroidBrowserOverlay111Page? browserOverlay = null;

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    browserOverlay = AndroidBrowserOverlay111Page.AssertInBrowserOverlay(driver);
                    browserOverlay
                        .AssertOnPage()
                        .ReturnToApp();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    browserOverlay?.AssertNoInternet();
                });

            var connectionErrorPage = AndroidBackToHomeConnectionErrorPage
                .AssertOnPage(driver);

            // Android device seems to at this point take a bit longer to work out there is an internet connection
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(10));

            connectionErrorPage
                .BackToHome();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
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
                .GoTo111();

            // Need to reuse the same page instance in the known issue fallback assertion
            // as creating a new one will result in a new context being grabbed.
            IOSBrowserOverlay111Page? browserOverlay = null;

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    browserOverlay = IOSBrowserOverlay111Page.AssertInBrowserOverlay(driver);
                    browserOverlay
                        .AssertOnPage()
                        .ReturnToApp();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    browserOverlay?.AssertNoInternet();
                });

            IOSBackToHomeConnectionErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}
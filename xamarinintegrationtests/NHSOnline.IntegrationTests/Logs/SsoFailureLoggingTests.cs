using System.Text.Encodings.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.HttpMocks.WebIntegrations;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Logs
{
    [TestClass]
    public class SsoFailureLoggingTests
    {
        [NhsAppAndroidTest]
        public void ASingleSignOnFlowThatEndsWithTheNhsLoginEmailPromptResultsInLogsThatCanBeMonitoredAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Dirk").FamilyName("Schubert"))
                .WithBehaviour(new RedirectToEnterEmailVaccineRecordSsoBehaviour());
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            var timing = TimedTestExecutor.Execute(() =>
            {
                AndroidYourHealthPage
                    .AssertOnPage(driver)
                    .PageContent.NavigateToVaccineRecord();

                AndroidNhsLoginSingleSignOnEnterEmailPage
                    .AssertOnPage(driver);
            });

            var clientLogs = new ClientLoggerLogs(timing.StartTime, timing.StopTime);
            clientLogs.AssertClientLogPresent(@"Web integration page load ended up on NHS login enter-email screen. Redirect flow was:
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-1
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-2
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-3
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-4
http://auth.nhslogin.stubs.local.bitraft.io:8080/enter-email");
        }

        [NhsAppIOSTest]
        public void ASingleSignOnFlowThatEndsWithTheNhsLoginEmailPromptResultsInLogsThatCanBeMonitoredIos(
            IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Schubert").FamilyName("Dirklington"))
                .WithBehaviour(new RedirectToEnterEmailVaccineRecordSsoBehaviour());
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            var timing = TimedTestExecutor.Execute(() =>
            {
                IOSYourHealthPage
                    .AssertOnPage(driver)
                    .PageContent.NavigateToVaccineRecord();

                IOSNhsLoginSingleSignOnEnterEmailPage
                    .AssertOnPage(driver);
            });

            var clientLogs = new ClientLoggerLogs(timing.StartTime, timing.StopTime);
            clientLogs.AssertClientLogPresent(@"Web integration page load ended up on NHS login enter-email screen. Redirect flow was:
http://nhsd.stubs.local.bitraft.io:8080/sso
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-1
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-2
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-3
http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/redirect-chain-4
http://auth.nhslogin.stubs.local.bitraft.io:8080/enter-email");
        }
    }
}

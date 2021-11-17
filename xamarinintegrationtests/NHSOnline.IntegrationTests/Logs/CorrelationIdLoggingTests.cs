using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Logs
{
    [TestClass]
    public class CorrelationIdLoggingTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsCorrelationIdAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            var testTiming = TimedTestExecutor.Execute(() =>
            {
                AndroidStubbedLoginPageSlimHeader
                    .AssertOnPage(driver)
                    .PageContent
                    .Login(patient);

                AndroidTermsAndConditionsPage
                    .AssertOnPage(driver)
                    .PageContent.AcceptTermsAndConditions();
            });

            SessionCorrelationIdDockerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs")
                .AssertFound();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsCorrelationIdIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            var testTiming = TimedTestExecutor.Execute(() =>
            {
                IOSStubbedLoginPage
                    .AssertOnPage(driver)
                    .PageContent.Login(patient);

                IOSTermsAndConditionsPage
                    .AssertOnPage(driver)
                    .PageContent.AcceptTermsAndConditions();
            });

            SessionCorrelationIdDockerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs")
                .AssertFound();
        }
    }
}
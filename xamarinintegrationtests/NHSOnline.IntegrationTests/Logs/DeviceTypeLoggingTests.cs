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
    [BusinessRule("BR-LOG-01.12", "Launching the app logs the user's device details")]
    public class DeviceTypeLoggingTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsUserAgentInformationAndroid(IAndroidDriverWrapper driver)
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

            UserAgentDockerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs",
                    Platform.Android)
                .AssertFound();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsUserAgentInformationIOS(IIOSDriverWrapper driver)
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

            UserAgentDockerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs",
                    Platform.Ios)
                .AssertFound();
        }
    }
}
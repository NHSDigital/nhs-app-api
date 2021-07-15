using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Logs
{
    [TestClass]
    [BusinessRule("BR-GEN-02.2", "Logging into the app logs device referrer information")]
    public class InstallReferrerLoggingTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsTheInstallReferrerAndroid(IAndroidDriverWrapper driver)
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
                AndroidStubbedLoginPage
                    .AssertOnPage(driver)
                    .PageContent
                    .AssertVectorOfTrust()
                    .Login(patient);

                AndroidTermsAndConditionsPage
                    .AssertOnPage(driver)
                    .PageContent.AcceptTermsAndConditions();
            });

            InstallReferrerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs",
                    patient,
                    patient.OdsCode)
                .AssertFound();
        }
    }
}
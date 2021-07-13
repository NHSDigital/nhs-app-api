using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Logs
{
    [TestClass]
    [BusinessRule("BR-GEN-02.1", "Launching the app logs device referrer information")]
    [BusinessRule("BR-GEN-02.2", "Logging into the app logs device referrer information")]
    public class InstallReferrerLoggingTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndLogsTheInstallReferrerAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            var testTiming = TimedTestExecutor.Execute(()
                => LoginProcess.LogAndroidPatientIn(driver, patient));

            InstallReferrerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs",
                    patient,
                    patient.OdsCode)
                .AssertFound();
        }
    }
}
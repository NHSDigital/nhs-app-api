using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
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

            var testTiming = TimedTestExecutor.Execute(()
                => LoginProcess.LogAndroidPatientIn(driver, patient));

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

            var testTiming = TimedTestExecutor.Execute(()
                => LoginProcess.LogIOSPatientIn(driver, patient));

            UserAgentDockerLogs.GetLogs(testTiming.StartTime,
                    testTiming.StopTime,
                    "pfs",
                    Platform.Ios)
                .AssertFound();
        }
    }
}
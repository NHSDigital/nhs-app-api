using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestContext
    {
        private readonly ITestMethod _testMethod;
        private readonly TestLogs _logs;
        private readonly IDriverWrapper _driver;

        internal TestContext(ITestMethod testMethod, TestLogs logs, IDriverWrapper driver)
        {
            _testMethod = testMethod;
            _logs = logs;
            _driver = driver;
        }

        internal string TestName => _testMethod.TestMethodName;
        internal DateTime StartTime { get; } = DateTime.UtcNow;
        internal TestLogs Logs => _logs;

        internal void Cleanup(TestResult testResult)
        {
            var testResultContext = new TestResultContext(this, testResult);

            if (testResult.Outcome != UnitTestOutcome.Passed)
            {
                _driver.AttachDebugInfo(testResultContext);
                AttachDockerLogs(testResultContext);
            }

            _driver.Cleanup(testResultContext);
        }

        private void AttachDockerLogs(TestResultContext testResultContext)
        {
            using var listContainerNames = new ProcessRunner("docker", "ps -a --filter=network=int_test_default --format {{.Names}}").Start();

            foreach (var containerName in listContainerNames.Output())
            {
                testResultContext.TryCleanUp(
                    $"attach docker logs {containerName}",
                    () => AttachDockerLogs(testResultContext, containerName));
            }
        }

        private void AttachDockerLogs(TestResultContext testResultContext, string containerName)
        {
            var filename = Path.Join(Path.GetTempPath(), $"{containerName}.log");
            File.Delete(filename);

            var arguments = $"logs {containerName} --since {StartTime:yyyy-MM-ddTHH:mm:ssZ}";

            // Add timestamps to the web logs as they are not included in the log lines
            if (containerName.Contains("web", StringComparison.OrdinalIgnoreCase))
            {
                arguments += " --timestamps";
            }

            using var logs = new ProcessRunner("docker", arguments).Start();
            File.AppendAllLines(filename, logs.Output());

            if (File.Exists(filename))
            {
                testResultContext.AddResultFile(filename);
            }
        }
    }
}
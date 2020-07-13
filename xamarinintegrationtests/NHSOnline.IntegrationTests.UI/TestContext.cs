using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestContext
    {
        private readonly DateTime _startTime = DateTime.UtcNow;

        private readonly ITestMethod _testMethod;
        private readonly TestTempDirectory _testTempDirectory;
        private readonly IDriverWrapper _driver;

        internal TestContext(
            ITestMethod testMethod,
            TestLogs logs,
            TestTempDirectory testTempDirectory,
            IDriverWrapper driver)
        {
            _testMethod = testMethod;
            _testTempDirectory = testTempDirectory;
            _driver = driver;

            Logs = logs;
        }

        internal TestLogs Logs { get; }

        internal string TestName => _testMethod.TestMethodName;

        internal FileInfo GetTempFile(string filename) => _testTempDirectory.GetTempFile(filename);

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
            var file = _testTempDirectory.GetTempFile($"{containerName}.log");
            
            var arguments = $"logs {containerName} --since {_startTime:yyyy-MM-ddTHH:mm:ssZ}";

            // Add timestamps to the web logs as they are not included in the log lines
            if (containerName.Contains("web", StringComparison.OrdinalIgnoreCase))
            {
                arguments += " --timestamps";
            }

            using var logs = new ProcessRunner("docker", arguments).Start();
            File.AppendAllLines(file.FullName, logs.Output());

            if (file.Exists)
            {
                testResultContext.Attach(file);
            }
        }
    }
}